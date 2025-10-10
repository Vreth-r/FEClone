/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// now holds the string to terraintile mapping?
// is this needed?

[CreateAssetMenu(menuName = "Tactics RPG/Terrain Database")]
public class TerrainDatabase : Database<TerrainTile>
{
    public static TerrainDatabase Instance;
    private Dictionary<string, TerrainTile> tileByName; // this is an extra dict for a different inq

    public void Init()
    {
        base.Initialize();
        if (Instance == null) Instance = this;
        tileByName = new Dictionary<string, TerrainTile>();
        foreach (var terrain in allData)
        {
            tileByName[terrain.terrainName] = terrain;
        }
    }

    public TerrainTile GetTerrainForTile(TileBase tile)
    {
        if (tile == null) return null;
        tileByVisual.TryGetValue(tile, out var result);
        return result;
    }
}
*/
