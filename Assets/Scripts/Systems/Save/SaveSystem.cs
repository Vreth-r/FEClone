using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SaveFolder => Application.persistentDataPath + "/Saves/";

    public static void SaveGame(SaveData data, int slot = 0)
    {
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        string path = SaveFolder + $"save_{slot}.json";
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log("Game Saved to " + path);
    }

    public static SaveData LoadGame(int slot = 0)
    {
        string path = SaveFolder + $"save_{slot}.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game loaded from " + path);
            return data;
        }

        Debug.LogWarning("No save file found at " + path);
        return null;
    }

    public static bool SaveExists(int slot = 0)
    {
        return File.Exists(SaveFolder + $"save_{slot}.json");
    }
}