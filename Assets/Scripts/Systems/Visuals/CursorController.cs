using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CursorController : MonoBehaviour
{
    [Header("Cursor Movement")]
    public float moveCooldown = 0.05f;
    private float lastMoveTime;
    private Vector3Int currentGridPosition;

    [Header("Tilemap & Tile Settings")]
    public Tilemap cursorTilemap;
    public Tilemap terrainTilemap;
    public Collider2D boundsCollider;
    public TileBase cursorTile;
    public Grid grid;
    private Vector3 minCursorPos;
    private Vector3 maxCursorPos;
    private bool targetMode = false;

    [Header("Tile Info Display")]
    public TileInfo tileInfoDisplay;
    public UnitInfo unitInfoDisplay;

    public static CursorController Instance; // this could probably be not singleton maybe idk check later

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        // semantically there will always be an Ylru to latch onto (hardcoding at its finest)
        currentGridPosition = (Vector3Int)UnitManager.Instance.GetUnitPositionByName("Ylru");
        LoadGridBounds();
        UpdateCursorTile();
    }

    void OnEnable()
    {
        ControlsManager.Instance.OnSelect += HandleSelect;
        ControlsManager.Instance.OnContextSwitch += HandleContextSwitch;
    }

    void OnDisable() // this may not be a neccessary function
    {
        ControlsManager.Instance.OnSelect -= HandleSelect;
        ControlsManager.Instance.OnContextSwitch -= HandleContextSwitch;
    }

    public void LoadGridBounds() // might make an interface for this
    {
        //Bounds mapBounds = terrainTilemap.localBounds;
        Bounds mapBounds = boundsCollider.bounds;
        // clamp
        minCursorPos = mapBounds.min;
        maxCursorPos = mapBounds.max - new Vector3(1, 1, 0); // unknown why but without this the cursor can go *just* one over the tilemap out of bounds
    }

    void Update()
    {
        if (ControlsManager.Instance.CurrentContext != InputContext.Gameplay)
            return;

        Vector2 input = ControlsManager.Instance.MoveInput;

        if (Time.time - lastMoveTime < moveCooldown || input == Vector2.zero)
            return;

        // if in targeting mode, cycle targets instead of moving
        if (targetMode)
        {
            int direction = 0;
            if (input.x > 0.5f || input.y > 0.5f)
                direction = 1;
            else if (input.x < -0.5f || input.y < -0.5f)
                direction = -1;

            if (direction != 0)
            {
                TargetSelector.Instance.CycleTarget(direction);
                lastMoveTime = Time.time;
            }
            return;
        }

        Vector2Int directionVec = Vector2Int.zero;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            directionVec.x = (int)Mathf.Sign(input.x);
        else
            directionVec.y = (int)Mathf.Sign(input.y);

        currentGridPosition += new Vector3Int(directionVec.x, directionVec.y, 0);
        Vector3Int clampedPosition = new Vector3Int(
        (int)Mathf.Clamp(currentGridPosition.x, minCursorPos.x, maxCursorPos.x),
        (int)Mathf.Clamp(currentGridPosition.y, minCursorPos.y, maxCursorPos.y),
        0
        );
        currentGridPosition = clampedPosition;
        UpdateCursorTile();
        AudioEvents.Play("cursor_tile_move");
        lastMoveTime = Time.time;
    }

    void HandleSelect()
    {
        if (ControlsManager.Instance.CurrentContext != InputContext.Gameplay)
            return;

        var unit = UnitManager.Instance.GetUnitAt((Vector2Int)currentGridPosition);
        if (unit != null)
        {
            unit.GetComponent<UnitMovement>().enabled = true;
            unit.GetComponent<UnitMovement>().SelectUnit(); // blocking enemy selection is baked into method, a little wasteful but it makes this look nice and clean
            unit.GetComponent<UnitMovement>().EnableControls(); // this is my temp solution
        }
        AudioEvents.Play("cursor_tile_click");
    }

    void HandleContextSwitch(InputContext newContext)
    {
        switch(newContext) 
        {
            case InputContext.Gameplay:
                cursorTilemap.gameObject.SetActive(true);
                UpdateInfoDisplays();
                break;
            case InputContext.Cutscene:
                cursorTilemap.gameObject.SetActive(false);
                tileInfoDisplay.gameObject.SetActive(false);
                unitInfoDisplay.gameObject.SetActive(false);
                break;
        }
    }

    public void UpdateCursorTile()
    {
        cursorTilemap.ClearAllTiles(); // only one tile visible at a time
        cursorTilemap.SetTile(currentGridPosition, cursorTile);

        if (ControlsManager.Instance.CurrentContext != InputContext.Gameplay)
            return;

        UpdateInfoDisplays();
    }

    public void UpdateInfoDisplays()
    {
        TerrainTile terrain = GridManager.Instance.GetTerrainAt((Vector2Int)currentGridPosition);
        if (terrain == null)
        {
            tileInfoDisplay.gameObject.SetActive(false);
        }
        else
        {
            tileInfoDisplay.gameObject.SetActive(true);
            tileInfoDisplay.UpdateInfo(terrain.terrainName, terrain.moveCost);
        }

        Unit unitRef = UnitManager.Instance.GetUnitAt((Vector2Int)currentGridPosition);
        if (unitRef == null)
        {
            unitInfoDisplay.gameObject.SetActive(false);
        }
        else
        {
            unitInfoDisplay.gameObject.SetActive(true);
            unitInfoDisplay.UpdateInfo(unitRef.unitName, unitRef.currentHP, unitRef.maxHP);
        }
    }

    public Vector3Int GetCursorGridPosition()
    {
        return currentGridPosition;
    }

    public void SetCurrentGridPosition(Vector3Int pos)
    {
        currentGridPosition = pos;
    }

    public void SetTargetMode(bool mode)
    {
        targetMode = mode;
    }
}
