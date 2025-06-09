using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private static string SaveFolder => Application.persistentDataPath + "/Saves/";

    public static void SaveGame(int slot = 0)
    {
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        SaveData data = BuildSaveData();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(slot), json);

        Debug.Log($"Game saved to {GetPath(slot)}");
    }

    public static void LoadGame(int slot = 0)
    {
        string path = GetPath(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning("Save file not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        ApplySaveData(data);
    }

    public static bool SaveExists(int slot = 0)
    {
        return File.Exists(GetPath(slot));
    }

    private static string GetPath(int slot) => SaveFolder + $"save_{slot}.json";

    // build save data from current gamestate
    private static SaveData BuildSaveData()
    {
        SaveData data = new SaveData();
        data.sceneName = SceneManager.GetActiveScene().name;
        data.gold = GameManager.Instance.Gold;
        data.timestamp = System.DateTime.Now.ToString("g");

        data.savedUnits = new List<SavedUnitData>();
        foreach (var unit in UnitManager.Instance.GetAllUnits())
        {
            data.savedUnits.Add(ConvertUnitToSaved(unit));
        }

        data.convoyItemIDs = GameManager.Instance.convoy.ConvertAll(item => item.itemID);

        return data;
    }

    // Apply save to world
    private static void ApplySaveData(SaveData data)
    {
        SceneManager.LoadScene(data.sceneName);

        GameManager.Instance.Gold = data.gold;
        GameManager.Instance.convoy.Clear();
        foreach (string itemID in data.convoyItemIDs)
        {
            var item = ItemDatabase.Instance.GetByID(itemID);
            if (item != null) GameManager.Instance.convoy.Add(item);
        }

        // Delay until scene finishes loading
        SceneLoader.OnSceneLoaded += () =>
        {
            foreach (var unitData in data.savedUnits)
            {
                UnitSpawner.Instance.SpawnUnitFromSaveData(unitData, (Vector3Int)unitData.gridPosition);
            }
        };
    }

    // Convert unit to save data
    private static SavedUnitData ConvertUnitToSaved(Unit unit)
    {
        return new SavedUnitData
        {
            unitID = unit.unitName,
            unitClassName = unit.unitClass.className,
            level = unit.level,
            currentHP = unit.currentHP,
            maxHP = unit.maxHP,
            strength = unit.strength,
            arcane = unit.arcane,
            defense = unit.defense,
            speed = unit.speed,
            skill = unit.skill,
            resistance = unit.resistance,
            luck = unit.luck,
            inventoryIDs = unit.inventory.ConvertAll(item => item.itemID),
            equippedItemID = unit.equippedItem != null ? unit.equippedItem.itemID : null,
            gridPosition = unit.GridPosition
        };
    }
}