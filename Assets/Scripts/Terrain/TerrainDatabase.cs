using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Intended to hold all tile-to-terrain mappings for a single point of access code wise

[CreateAssetMenu(menuName = "Tactics RPG/Terrain Database")]
public class TerrainDatabase : ScriptableObject
{
    public TerrainDatabase Instance;
    [SerializeField] private List<TerrainTile> terrainTiles;

    private Dictionary<TileBase, TerrainTile> tileByVisual;
    private Dictionary<string, TerrainTile> tileByName;

    public void Initialize()
    {
        Debug.Log("TerrainDB: Init");
        tileByVisual = new Dictionary<TileBase, TerrainTile>();
        tileByName = new Dictionary<string, TerrainTile>();
        foreach (var terrain in terrainTiles)
        {
            tileByName[terrain.terrainName] = terrain;
            tileByVisual[terrain.tileVisual] = terrain;
        }
    }

    // this might be useless
    public void OnEnable()
    {
        if (Instance != null)
        {
            Instance = this;
            Initialize();
        }
    }

    public TerrainTile GetTerrainForTile(TileBase tile)
    {
        tileByVisual.TryGetValue(tile, out var result);
        return result;
    }

    public TileBase GetTileByName(string name)
    {
        return GetTerrainByName(name)?.tileVisual;
    }

    public TerrainTile GetTerrainByName(string name)
    {
        tileByName.TryGetValue(name, out var result);
        return result;
    }
}
