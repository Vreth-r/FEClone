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
    public TileBase cursorTile;
    public Grid grid;
    private Vector3 minCursorPos;
    private Vector3 maxCursorPos;

    public static CursorController Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentGridPosition = Vector3Int.zero;
        UpdateCursorTile();
    }

    void OnEnable()
    {
        ControlsManager.Instance.OnSelect += HandleSelect;
    }

    void OnDisable() // this may not be a neccessary function
    {
        ControlsManager.Instance.OnSelect -= HandleSelect;
    }

    public void LoadGridBounds() // might make an interface for this
    {
        Bounds mapBounds = terrainTilemap.localBounds;
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

        Vector2Int direction = Vector2Int.zero;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            direction.x = (int)Mathf.Sign(input.x);
        else
            direction.y = (int)Mathf.Sign(input.y);

        currentGridPosition += new Vector3Int(direction.x, direction.y, 0);
        Vector3Int clampedPosition = new Vector3Int(
        (int)Mathf.Clamp(currentGridPosition.x, minCursorPos.x, maxCursorPos.x),
        (int)Mathf.Clamp(currentGridPosition.y, minCursorPos.y, maxCursorPos.y),
        0
        );
        currentGridPosition = clampedPosition;
        UpdateCursorTile();
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
    }

    public void UpdateCursorTile()
    {
        cursorTilemap.ClearAllTiles(); // only one tile visible at a time
        cursorTilemap.SetTile(currentGridPosition, cursorTile);
    }

    public Vector3Int GetCursorGridPosition()
    {
        return currentGridPosition;
    }

    public void SetCurrentGridPosition(Vector3Int pos)
    {
        currentGridPosition = pos;
    }
}
