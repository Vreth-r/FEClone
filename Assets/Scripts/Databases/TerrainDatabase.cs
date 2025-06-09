using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Intended to hold all tile-to-terrain mappings for a single point of access code wise

[CreateAssetMenu(menuName = "Tactics RPG/Terrain Database")]
public class TerrainDatabase : Database<TerrainTile>
{
    public static TerrainDatabase Instance;
    private Dictionary<TileBase, TerrainTile> tileByVisual;

    public void Init()
    {
        base.Initialize();
        if (Instance == null) Instance = this;
        tileByVisual = new Dictionary<TileBase, TerrainTile>();
        foreach (var terrain in allData)
        {
            tileByVisual[terrain.tileVisual] = terrain;
        }
    }

    public TerrainTile GetTerrainForTile(TileBase tile)
    {
        if (tile == null) return null;
        tileByVisual.TryGetValue(tile, out var result);
        return result;
    }

    public TileBase GetTileByName(string name)
    {
        return GetByID(name).tileVisual;
    }
}
