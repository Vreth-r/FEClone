using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cysharp.Threading.Tasks;

public enum TargetingMode
{
    None,
    Units,
    Tiles
}

public class TargetSelector : MonoBehaviour
{
    public static TargetSelector Instance;

    private Unit attacker;
    private List<Unit> validUnitTargets = new();
    private int currentTargetIndex = 0;

    private bool targeting = false;
    private TargetingMode currentMode = TargetingMode.None;

    public Tilemap highlightTilemap;
    public TileBase targetHighlightTile;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        ControlsManager.Instance.OnSelect += HandleSelect;
    }

    private void OnDisable()
    {
        ControlsManager.Instance.OnSelect -= HandleSelect;
    }

    private void Update()
    {
        if (!targeting) return;

        Vector2 moveInput = ControlsManager.Instance.MoveInput;
        if (Mathf.Abs(moveInput.x) > 0.5f)
        {
            int dir = moveInput.x > 0 ? 1 : -1;
            CycleTarget(dir);
        }
    }

    private async void HandleSelect()
    {
        if (!targeting) return;

        switch (currentMode)
        {
            case TargetingMode.Units:
                await HandleUnitSelection();
                break;

            case TargetingMode.Tiles:
                await HandleTileSelection();
                break;
        }
    }

    private async UniTask HandleUnitSelection()
    {
        if (validUnitTargets.Count == 0) return;

        Unit target = validUnitTargets[currentTargetIndex];
        if (attacker == null || target == null) return;

        await CombatSystem.StartCombat(attacker, target);
        attacker.state = UnitState.Tapped;
        Clear();
        TurnManager.Instance.TryEndPlayerTurn();
    }

    private async UniTask HandleTileSelection()
    {
        Vector3Int cursorPos = CursorController.Instance.GetCursorGridPosition();
        Vector2Int gridPos = new(cursorPos.x, cursorPos.y);

        Debug.Log($"Tile selected at {gridPos}");
        // TODO: Return tile or execute callback here
        await UniTask.Yield();

        Clear();
    }

    // === Entry Points ===
    public void BeginTargetingUnits(Unit unit)
    {
        Clear();
        attacker = unit;
        currentMode = TargetingMode.Units;

        validUnitTargets.Clear();

        if (attacker.equippedItem is not WeaponItem weapon)
        {
            Debug.Log("No weapon equipped");
            return;
        }

        foreach (var pos in UnitManager.Instance.GetAllOccupiedPositions())
        {
            Unit other = UnitManager.Instance.GetUnitAt(pos);
            if (other == null || other.team != Team.Enemy) continue;

            int dist = Mathf.Abs(unit.GridPosition.x - pos.x) + Mathf.Abs(unit.GridPosition.y - pos.y);
            if (dist >= weapon.minRange && dist <= weapon.maxRange)
            {
                validUnitTargets.Add(other);
                highlightTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), targetHighlightTile);
            }
        }

        if (validUnitTargets.Count == 0)
        {
            Debug.Log("No enemies in range.");
            attacker = null;
            return;
        }

        Debug.Log("Select an enemy to attack.");
        targeting = true;
        CursorController.Instance.SetTargetMode(true);

        FocusTarget(0);
    }

    public void BeginTargetingTiles(Unit unit)
    {
        Clear();
        attacker = unit;
        currentMode = TargetingMode.Tiles;

        Debug.Log("Select a tile.");
        targeting = true;
        CursorController.Instance.SetTargetMode(true);
    }

    // === Internal Helpers ===
    public void CycleTarget(int direction)
    {
        if (!targeting || currentMode != TargetingMode.Units || validUnitTargets.Count == 0)
            return;

        currentTargetIndex += direction;
        if (currentTargetIndex >= validUnitTargets.Count)
            currentTargetIndex = 0;
        else if (currentTargetIndex < 0)
            currentTargetIndex = validUnitTargets.Count - 1;

        FocusTarget(currentTargetIndex);
    }

    private void FocusTarget(int index)
    {
        if (index < 0 || index >= validUnitTargets.Count) return;

        Unit target = validUnitTargets[index];
        CursorController.Instance.SetCurrentGridPosition(
            new Vector3Int(target.GridPosition.x, target.GridPosition.y, 0)
        );
        CursorController.Instance.UpdateCursorTile();
    }

    public void Clear()
    {
        highlightTilemap.ClearAllTiles();
        attacker?.GetComponent<MovementRange>()?.ClearHighlights();

        attacker = null;
        validUnitTargets.Clear();
        targeting = false;
        currentTargetIndex = 0;
        currentMode = TargetingMode.None;

        CursorController.Instance.SetTargetMode(false);
    }

    // === Future Async API ===
    public async UniTask<Unit> AwaitUnitTargetAsync(Unit user)
    {
        BeginTargetingUnits(user);
        await UniTask.WaitUntil(() => !targeting);
        return validUnitTargets.Count > 0 ? validUnitTargets[currentTargetIndex] : null;
    }

    public async UniTask<Vector2Int> AwaitTileTargetAsync(Unit user)
    {
        BeginTargetingTiles(user);
        Vector2Int selected = Vector2Int.zero;
        await UniTask.WaitUntil(() => !targeting);
        return selected;
    }
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cysharp.Threading.Tasks;

// waiting to document this one i still have to test and tweak

public class TargetSelector : MonoBehaviour
{
    public static TargetSelector Instance;

    private Unit attacker;
    private List<Unit> validTargets = new();
    private int currentTargetIndex = 0;
    private bool targeting = false;

    public Tilemap highlightTilemap;
    public TileBase targetHighlightTile;

    private void Awake()
    {
        Instance = this; // Singleton pattern my beloved
    }

    void OnEnable()
    {
        ControlsManager.Instance.OnSelect += HandleSelect;
        //ControlsManager.Instance.OnCancel += HandleCancel;
    }

    void OnDisable()
    {
        ControlsManager.Instance.OnSelect -= HandleSelect;
        //ControlsManager.Instance.OnCancel -= HandleCancel;
    }

    private async void HandleSelect()
    {
        if (!targeting) return;

        // get the current cursor position
        Vector3Int cursorPos = CursorController.Instance.GetCursorGridPosition();
        Vector2Int gridPos = new(cursorPos.x, cursorPos.y);

        foreach (var target in validTargets)
        {
            if (target.GridPosition == gridPos)
            {
                await CombatSystem.StartCombat(attacker, target);
                attacker.state = UnitState.Tapped;
                Clear();
                TurnManager.Instance.TryEndPlayerTurn();
                return;
            }
        }

        Debug.Log("Invalid Target");
    }

    private void HandleCancel()
    {
        if (!targeting) return;
        Clear();
    }

    public void BeginTargeting(Unit unit)
    {
        attacker = unit;
        validTargets.Clear();

        WeaponItem weapon = attacker.equippedItem as WeaponItem;
        if (weapon == null)
        {
            Debug.Log("No weapon equipped");
            return;
        }

        foreach (var pos in UnitManager.Instance.GetAllOccupiedPositions())
        {
            Unit other = UnitManager.Instance.GetUnitAt(pos);
            if (other == null || other.team != Team.Enemy) continue;

            int dist = Mathf.Abs(unit.GridPosition.x - pos.x) + Mathf.Abs(unit.GridPosition.y - pos.y);
            if (dist >= weapon.minRange && dist <= weapon.maxRange)
            {
                validTargets.Add(other);

                // highlight enemy tile
                highlightTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), targetHighlightTile);
            }
        }

        if (validTargets.Count == 0)
        {
            Debug.Log("No enemies in range.");
            attacker = null;
            return;
        }

        Debug.Log("Select an enemy to attack.");
        targeting = true; // enable targeting mode
        CursorController.Instance.SetTargetMode(true);

        CursorController.Instance.SetCurrentGridPosition(new Vector3Int(validTargets[0].GridPosition.x, validTargets[0].GridPosition.y, 0));
        CursorController.Instance.UpdateCursorTile();
    }

    public void CycleTarget(int direction)
    {
        if (!targeting || validTargets.Count == 0) return;
        currentTargetIndex += direction;
        if (currentTargetIndex >= validTargets.Count)
            currentTargetIndex = 0;
        else if (currentTargetIndex < 0)
            currentTargetIndex = validTargets.Count - 1;

        CursorController.Instance.SetCurrentGridPosition(new Vector3Int(validTargets[currentTargetIndex].GridPosition.x, validTargets[currentTargetIndex].GridPosition.y, 0));
        CursorController.Instance.UpdateCursorTile();
    }

    public void Clear()
    {
        attacker.GetComponent<MovementRange>().ClearHighlights();
        attacker = null;
        validTargets.Clear();
        targeting = false;
        currentTargetIndex = 0;
        CursorController.Instance.SetTargetMode(false);
    }
}
*/
