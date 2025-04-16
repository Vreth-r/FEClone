using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// purpose is to track occupied tiles so you dont have to reference 30 different units
public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private readonly HashSet<Vector2Int> occupiedTiles = new();

    private void Awake() => Instance = this;

    public void RegisterUnit(Unit unit)
    {
        occupiedTiles.Add(unit.GridPosition);
    }

    public void UnregisterUnit(Unit unit)
    {
        occupiedTiles.Remove(unit.GridPosition);
    }

    public void UpdateUnitPosition(Unit unit, Vector2Int oldPos, Vector2Int newPos)
    {
        occupiedTiles.Remove(oldPos);
        occupiedTiles.Add(newPos);
    }

    public bool IsOccupied(Vector2Int pos) => occupiedTiles.Contains(pos);
}
