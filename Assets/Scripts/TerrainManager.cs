using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager Instance;

    [SerializeField] private TerrainDatabase terrainDatabase;
    [SerializeField] private Tilemap tilemap;

    private void Awake()
    {
        Instance = this;
    }

    public TerrainTile GetTerrainAt(Vector2Int gridPos)
    {
        Vector3Int cell = new(gridPos.x, gridPos.y, 0);
        TileBase tile = tilemap.GetTile(cell);
        return terrainDatabase.GetTerrainForTile(tile);
    }
}
