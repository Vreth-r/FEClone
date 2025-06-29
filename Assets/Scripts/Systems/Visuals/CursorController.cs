using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CursorController : MonoBehaviour
{
    [Header("Cursor Movement")]
    public float moveCooldown = 0.2f;
    private float lastMoveTime;
    private Vector3Int currentGridPosition;

    [Header("Tilemap & Tile Settings")]
    public Tilemap cursorTilemap;
    public TileBase cursorTile;
    public Grid grid;

    void Start()
    {
        currentGridPosition = Vector3Int.zero;
        UpdateCursorTile();
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

        /*
        Vector3Int clampedPosition = new Vector3Int(
        Mathf.Clamp(currentGridPosition.x, minX, maxX),
        Mathf.Clamp(currentGridPosition.y, minY, maxY),
        0
        );
        currentGridPosition = clampedPosition;
        */
        currentGridPosition += new Vector3Int(direction.x, direction.y, 0);
        UpdateCursorTile();
        lastMoveTime = Time.time;
    }

    void UpdateCursorTile()
    {
        cursorTilemap.ClearAllTiles(); // only one tile visible at a time
        cursorTilemap.SetTile(currentGridPosition, cursorTile);
    }

    public Vector3Int GetCursorGridPosition()
    {
        return currentGridPosition;
    }
}
