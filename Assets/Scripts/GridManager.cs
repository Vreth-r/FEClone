using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public Tilemap tilemap;
    public Vector2Int gridSize;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3Int WorldToCell(Vector3 worldPos) => tilemap.WorldToCell(worldPos);
    public Vector3 CellToWorld(Vector3Int cellPos) => tilemap.CellToWorld(cellPos);
}
