using UnityEngine;
using System.Collections.Generic;
using System.IO;

// To be attached to an editor only game object 
// for easily building levels in unity then exporting them to json

public class MapEncoder : MonoBehaviour
{
    public string outputFileName = "NewMap";
    public string exportPath = "Assets/Maps/";

    public void ExportCurrentMap()
    {
        LevelMapData mapData = new LevelMapData();
        var grid = GridManager.Instance;

        mapData.mapID = outputFileName;
        mapData.displayName = "Export Level"; // can be changed manually

        mapData.tiles = new List<TileData>();
        BoundsInt bounds = grid.tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            var tile = grid.GetTerrainAt((Vector2Int)pos);
            if (tile != null)
            {
                mapData.tiles.Add(new TileData
                {
                    x = pos.x,
                    y = pos.y,
                    terrainType = tile.terrainName
                });
            }
        }

        mapData.playerUnits = new();
        mapData.enemyUnits = new();

        foreach (var unit in UnitManager.Instance.GetAllUnits())
        {
            var spawn = new UnitSpawnData
            {
                unitID = unit.unitName,
                x = unit.GridPosition.x,
                y = unit.GridPosition.y,
                isPlayer = unit.team == Team.Player
            };

            if (spawn.isPlayer)
                mapData.playerUnits.Add(spawn);
            else
                mapData.enemyUnits.Add(spawn);
        }

        string json = JsonUtility.ToJson(mapData, true);
        File.WriteAllText(Path.Combine(exportPath, outputFileName + ".json"), json);
        Debug.Log($"Exported map to {exportPath + outputFileName}.json");
    }

    /* place this in a helper
    public static LevelMapData LoadMapData(string fileName)
{
    string fullPath = Path.Combine(Application.streamingAssetsPath, "Maps", fileName + ".json");
    if (!File.Exists(fullPath))
    {
        Debug.LogError("Map file not found: " + fullPath);
        return null;
    }

    string json = File.ReadAllText(fullPath);
    return JsonUtility.FromJson<LevelMapData>(json);
}
*/
}