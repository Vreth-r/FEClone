using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// waiting to document this one i still have to test and tweak

public class TargetSelector : MonoBehaviour
{
    public static TargetSelector Instance;

    private Unit attacker;
    private List<Unit> validTargets = new();
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
    }

    void OnDisable()
    {
        ControlsManager.Instance.OnSelect -= HandleSelect;
    }

    private void HandleSelect()
    {
        if (!targeting) return;

        // get the current cursor position
        Vector3Int cursorPos = CursorController.Instance.GetCursorGridPosition();
        Vector2Int gridPos = new(cursorPos.x, cursorPos.y);

        foreach (var target in validTargets)
        {
            if (target.GridPosition == gridPos)
            {
                CombatSystem.StartCombat(attacker, target);
                attacker.state = UnitState.Tapped;
                Clear();
                return;
            }
        }

        Debug.Log("Invalid Target");
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
    }
    
    public void Clear()
    {
        attacker.GetComponent<MovementRange>().ClearHighlights();
        attacker = null;
        validTargets.Clear();
        targeting = false;
    }
}
