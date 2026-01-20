[System.Serializable]
public class UnitRosterEntry
{
    public Unit UnitPrefab;
    public UnitRuntimeState RuntimeState;
    public bool IsAlive = true;

    public string UnitID => UnitPrefab.unitID;

    public UnitRosterEntry(Unit unitPrefab)
    {
        UnitPrefab = unitPrefab;
        RuntimeState = unitPrefab.ExtractRuntimeState();
    }
}