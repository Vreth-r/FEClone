using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// purpose is to track occupied tiles so you dont have to reference 30 different units
public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private readonly Dictionary<Vector2Int, Unit> unitPositions = new(); // keeps track of occupied tiles by all units

    public Unit selectedUnit; // for use in keeping track what unit is selected so others cant be selected at the same time

    private void Awake() => Instance = this; // declare this instance for external ref

    public void RegisterUnit(Unit unit)
    {
        unitPositions[unit.GridPosition] = unit; // track this unit 
    }

    public void UnregisterUnit(Unit unit)
    {
        unitPositions.Remove(unit.GridPosition); // stop tracking unit (cause it died lmao)
    }

    public void UpdateUnitPosition(Unit unit, Vector2Int oldPos, Vector2Int newPos)
    {
        // get rid of old position and add new position, not much other info is needed but may be expanded on in the future for mechanics
        unitPositions.Remove(oldPos); 
        unitPositions[newPos] = unit;
    }

    public bool IsOccupied(Vector2Int pos) => unitPositions.ContainsKey(pos); // checks if a tile is occupied

    public Unit GetUnitAt(Vector2Int pos)
    {
        unitPositions.TryGetValue(pos, out Unit unit);
        return unit;
    }

    public IEnumerable<Vector2Int> GetAllOccupiedPositions()
    {
        return unitPositions.Keys;
    }

    public IEnumerable<Unit> GetAllUnits()
    {
        return unitPositions.Values;
    }

    public void selectUnit(Unit unit)
    {
        selectedUnit = unit; // selected a unit
    }

    public void deselectedUnit()
    {
        selectedUnit = null; // voids selected unit
    }

    public bool isUnitSelected(Unit unit)
    {
        return selectedUnit == unit; // returns if the provided unit is selected
    }

    public bool isAUnitSelected()
    {
        return selectedUnit != null; // returns if there is a selected unit
    }
}
