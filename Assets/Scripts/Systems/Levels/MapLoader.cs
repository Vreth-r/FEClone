using UnityEngine;
using System.IO;

/// <summary>
///  Class <c>MapLoader</c> loads all terrain and unit information for a level from its json file.
/// </summary>
public class MapLoader : MonoBehaviour
{
    [Header("References")]
    public UnitSpawner unitSpawner; // the unit spawner is to be attatched to the game manager

    [Header("Data")]
    public string mapFileName; // for dev reasons will be set in editor for now

    public void LoadMap(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Maps", fileName + ".json"); // find the file
        if (!File.Exists(path))
        {
            Debug.LogError("Map file not found: " + path); // error handling
            return;
        }

        string json = File.ReadAllText(path);
        LevelMapData mapData = JsonUtility.FromJson<LevelMapData>(json); // grab json reference

        if (mapData == null)
        {
            Debug.LogError("Failed to parse map data");
            return;
        }

        BuildMap(mapData); // begin parsing
    }

    public void LoadFromField() => LoadMap(mapFileName);

    public void BuildMap(LevelMapData data)
    {
        // clear current state
        GridManager.Instance.ClearGrid();
        UnitManager.Instance.ClearAllUnits();

        GridManager.Instance.Initialize();

        // load terrain from CSV
        if (data.tileCSV != null && data.terrainKey != null)
        {
            string[] rows = data.tileCSV.Split('\n');
            for (int y = 0; y < rows.Length; y++)
            {
                string[] cells = rows[y].Trim().Split(',');
                for (int x = 0; x < cells.Length; x++)
                {
                    if (int.TryParse(cells[x], out int terrainIndex) && terrainIndex >= 0 && terrainIndex < data.terrainKey.Count)
                    {
                        string terrainType = data.terrainKey[terrainIndex];
                        GridManager.Instance.PlaceTerrain(x, y, terrainType);
                    }
                }
            }
        }
        else if (data.tiles != null && data.tiles.Count > 0)
        {
            // fallback to the uncompressed format
            foreach (TileData tile in data.tiles)
            {
                GridManager.Instance.PlaceTerrain(tile.x, tile.y, tile.terrainType);
            }
        }

        // place player units
        foreach (UnitSpawnData player in data.playerUnits)
        {
            UnitData unitData = UnitDatabase.Instance.GetUnitDataByID(player.unitID); // lmao i need to refactor databases
            unitSpawner.SpawnUnitFromTemplate(unitData, new Vector3Int(player.x, player.y, 0));
        }

        // place enemies
        foreach (UnitSpawnData enemy in data.enemyUnits)
        {
            UnitData unitData = UnitDatabase.Instance.GetUnitDataByID(enemy.unitID);
            var unit = unitSpawner.SpawnUnitFromTemplate(unitData, new Vector3Int(enemy.x, enemy.y, 0));
            unit.team = Team.Enemy;
        }

        Debug.Log($"Loaded map: {data.displayName}");
    }
}