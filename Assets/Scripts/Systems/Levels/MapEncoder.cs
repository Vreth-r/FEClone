using UnityEngine;
using System.Collections.Generic;
using System.IO;

// To be attached to an editor only game object 
// for easily building levels in unity then exporting them to json

public class MapEncoder : MonoBehaviour
{
    public string outputFileName = "NewMap";
    public string exportPath = "Assets/StreamingAssets/Maps";

    public void ExportCurrentMap()
    {
        LevelMapData mapData = new LevelMapData();
        var grid = GridManager.Instance;

        mapData.mapID = outputFileName;
        mapData.displayName = "Export Level";

        // build terrain index map
        Dictionary<string, int> terrainKeyMap = new();
        List<string> terrainKey = new();
        BoundsInt bounds = grid.tilemap.cellBounds;

        int width = bounds.size.x;
        int height = bounds.size.y;
        int[,] terrainIndices = new int[width, height];
        Debug.Log(width);
        Debug.Log(height);


        /*
            OK SO, unity places a small internal buffer for the actual bounding of tilemap grids for whatever forsaken reason,
            so this encoder can return whole rows of just null tiles, which is hilarious.
            Now, i dont plan on having null tiles the middle of the map, so ill just, not track them, but if you ever see something
            funny related to that, it's probably because of this.
        */
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int cellPos = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                var tile = grid.GetTerrainAt((Vector2Int)cellPos);
                int terrainIndex = -1;

                if (tile != null)
                {
                    if (!terrainKeyMap.TryGetValue(tile.terrainName, out terrainIndex))
                    {
                        terrainIndex = terrainKey.Count;
                        terrainKeyMap[tile.terrainName] = terrainIndex;
                        terrainKey.Add(tile.terrainName);
                    }
                    terrainIndices[x, y] = terrainIndex;
                }
            }
        }

        // Convert to CSV
        System.Text.StringBuilder csv = new();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                csv.Append(terrainIndices[x, y]);
                if (x < width - 1) csv.Append(',');
            }
            if (y < height - 1) csv.AppendLine();
        }

        mapData.terrainKey = terrainKey;
        mapData.tileCSV = csv.ToString();

        // Export units
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

        // export to JSON
        string json = JsonUtility.ToJson(mapData, true);
        File.WriteAllText(Path.Combine(exportPath, outputFileName + ".json"), json);
        Debug.Log($"MapEncoder: Exported map to {exportPath + outputFileName}.json");
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