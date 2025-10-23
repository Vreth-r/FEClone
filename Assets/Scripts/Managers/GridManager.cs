using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// bridge between the visual and data tilemap, if that's even...needed......
public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public Tilemap visualTilemap;
    public Tilemap dataTilemap;
    public Tilemap highlightTilemap;
    //public TerrainDatabase terrainDatabase;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3Int WorldToCell(Vector3 worldPos) => dataTilemap.WorldToCell(worldPos);
    public Vector3 CellToWorld(Vector3Int cellPos) => dataTilemap.CellToWorld(cellPos);

    /*
    public void ClearGrid()
    {
        tilemap.ClearAllTiles();
    }
    */

    // this is for later when loading already made levels
    public void Initialize()
    {
        //ClearGrid();
        // any other setup too
    }

    /* dont need this anymore if we are prebuilding levels
    public void PlaceTerrain(int x, int y, string terrainID)
    {
        TerrainTile data = terrainDatabase.GetByID(terrainID);
        if (data == null)
        {
            Debug.LogWarning($"Terrain type not found: {terrainID}");
            return;
        }

        tilemap.SetTile(new Vector3Int(x, y, 0), data.tileVisual);
    }
    */

    public TerrainTile GetTerrainAt(Vector2Int position)
    {
        TerrainTile tile = dataTilemap.GetTile(new Vector3Int(position.x, position.y, 0)) as TerrainTile; // YOU CAN DO THAT!?
        return tile;
    }
}
