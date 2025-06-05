using UnityEngine;
using System.IO;

public class MapLoader : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public UnitSpawner unitSpawner;

    [Header("Data")]
    public string mapFileName;

    public void LoadMap(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Maps", fileName + ".json");
        if (!File.Exists(path))
        {
            Debug.LogError("Map file not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        LevelMapData mapData = JsonUtility.FromJson<LevelMapData>(json);

        if (mapData == null)
        {
            Debug.LogError("Failed to parse map data");
            return;
        }

        BuildMap(mapData);
    }

    public void LoadFromField() => LoadMap(mapFileName);

    public void BuildMap(LevelMapData data)
    {
        // clear current state
        gridManager.ClearGrid();
        UnitManager.Instance.ClearAllUnits();

        gridManager.Initialize();

        //place terrain
        foreach (TileData tile in data.tiles)
        {
            gridManager.PlaceTerrain(tile.x, tile.y, tile.terrainType);
        }

        // place player units
        foreach (UnitSpawnData player in data.playerUnits)
        {
            UnitData unitData = UnitDatabase.GetUnitDataByID(player.unitID);
            unitSpawner.SpawnUnitFromTemplate(unitData, new Vector3Int(player.x, player.y, 0));
        }

        // place enemies
        foreach (UnitSpawnData enemy in data.enemyUnits)
        {
            UnitData unitData = UnitDatabase.GetUnitDataByID(enemy.unitID);
            var unit = unitSpawner.SpawnUnitFromTemplate(unitData, new Vector3Int(enemy.x, enemy.y, 0));
            unit.team = Team.Enemy;
        }

        Debug.Log($"Loaded map: {data.displayName}");
    }
}