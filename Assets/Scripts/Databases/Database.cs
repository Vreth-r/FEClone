using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// A generic database scriptable that can fit any type.
/// Uses a List to be able to populate the database easily in the Unity editor,
/// then dumps everything into a lookup dicitionary for O(1) access time.
/// </summary>
/// <typeparam name="T">The type the database will hold</typeparam>
public abstract class Database<T> : ScriptableObject where T : Object, IIdentifiable
{
    private Dictionary<string, T> lookup; // the lookup dicitonary
    public List<T> allData; // populated in the editor

    protected virtual void Initialize()
    {
        lookup = new Dictionary<string, T>();

        // Dump list data
        foreach (var data in allData)
        {
            if (data != null && !lookup.ContainsKey(data.ID))
            {
                lookup[data.ID] = data;
            }
        }
    }

    public T GetByID(string id)
    {
        if (lookup == null)
        {
            Debug.LogError($"{GetType().Name} not initialized.");
            return null;
        }

        if (lookup.TryGetValue(id, out var data))
            return data;

        Debug.LogWarning($"ID '{id}' not found in {GetType().Name}.");
        return null;
    }
}