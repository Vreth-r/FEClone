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
        return terrainDatabase.GetTerrainForTile(tilemap.GetTile(new(gridPos.x, gridPos.y, 0)));
    }

    public int GetTerrainCost(TerrainTile terrain)
    {
        return terrain.moveCost;
    }
}
