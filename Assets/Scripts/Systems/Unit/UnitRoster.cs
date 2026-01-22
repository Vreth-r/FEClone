using UnityEngine;
using System.Collections.Generic;

public class UnitRoster
{
    private readonly Dictionary<string, UnitRosterEntry> entries = new();

    public IReadOnlyCollection<UnitRosterEntry> Entries => entries.Values;

    public void Add(Unit unitPrefab)
    {
        if(!entries.ContainsKey(unitPrefab.unitID))
        {
            entries.Add(unitPrefab.unitID, new UnitRosterEntry(unitPrefab));
        }
    }

    public bool Contains(string unitID) => entries.ContainsKey(unitID);

    public UnitRosterEntry Get(string unitID)
    {
        entries.TryGetValue(unitID, out var entry);
        return entry;
    }

    public void DebugPrintThatShit()
    {
        foreach (var unit in entries)
        {
            Debug.Log($"{unit.Key}");
            Debug.Log(unit.Value);
        }
    }
}