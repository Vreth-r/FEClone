using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tactics RPG/Class Database")]
public class UnitClassDatabase : ScriptableObject
{
    public static UnitClassDatabase Instance;
    public List<UnitClass> classes;
    private Dictionary<string, UnitClass> classLookup;

    private void OnEnable()
    {
        Instance = this;
        classLookup = new Dictionary<string, UnitClass>();

        foreach (var c in classes)
        {
            if (c != null && !classLookup.ContainsKey(c.className))
            {
                classLookup[c.className] = c;
            }
        }
    }

    public static UnitClass GetClassByName(string name)
    {
        if (Instance == null || Instance.classLookup == null)
        {
            Debug.LogError("ClassDB not initialized");
            return null;
        }

        if (Instance.classLookup.TryGetValue(name, out var result)) return result;

        Debug.LogWarning($"Class {name} not found");
        return null;
    }
}