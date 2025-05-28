using UnityEngine;
using System.Collections.Generic;

// UnitClass Database, keeps a reference for each skill scriptable object

[CreateAssetMenu(menuName = "Tactics RPG/Class Database")]
public class UnitClassDatabase : ScriptableObject
{
    public static UnitClassDatabase Instance; // singleton reference
    public List<UnitClass> classes; // List for editor population
    private Dictionary<string, UnitClass> classLookup; // Dicitionary for O(1) reference

    private void OnEnable()
    {
        Instance = this; // assign singleton 
        classLookup = new Dictionary<string, UnitClass>(); // init dict

        foreach (var c in classes)
        {
            if (c != null && !classLookup.ContainsKey(c.className)) // if item exists and not already in the database
            {
                classLookup[c.className] = c; // add to DB
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