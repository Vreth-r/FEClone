using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Intended to hold all tile-to-terrain mappings for a single point of access code wise

[CreateAssetMenu(menuName = "Tactics RPG/Terrain Database")]
public class TerrainDatabase : ScriptableObject
{
    [SerializeField] private List<TerrainTile> terrainTiles;
    
    private Dictionary<TileBase, TerrainTile> _lookup;

    public void Initialize()
    {
        _lookup = new Dictionary<TileBase, TerrainTile>();
        foreach (var terrain in terrainTiles)
        {
            if(terrain.tileVisual != null && !_lookup.ContainsKey(terrain.tileVisual))
            {
                _lookup.Add(terrain.tileVisual, terrain);
            }
        }
    }

    public TerrainTile GetTerrainForTile(TileBase tile)
    {
        if (_lookup == null) Initialize();
        if(tile == null) return null;
        return _lookup.TryGetValue(tile, out var terrain) ? terrain : null;
    }
}
