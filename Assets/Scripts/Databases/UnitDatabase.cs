using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tactics RPG/Unit Database")]
public class UnitDatabase : ScriptableObject
{
    public List<UnitData> units;
    private Dictionary<string, UnitData> lookup;

    private void OnEnable()
    {
        lookup = new Dictionary<string, UnitData>();
        foreach (var u in units)
        {
            lookup[u.unitID] = u;
        }
    }

    public static UnitData GetUnitDataByID(string id)
    {
        if (Instance.lookup.TryGetValue(id, out var data))
            return data;

        Debug.LogWarning($"UnitData not found for ID: {id}");
        return null;
    }

    public static UnitDatabase Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}