using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// purpose is to track occupied tiles so you dont have to reference 30 different units
public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private readonly HashSet<Vector2Int> occupiedTiles = new(); // keeps track of occupied tiles by all units

    private void Awake() => Instance = this; // declare this instance for external ref

    public void RegisterUnit(Unit unit)
    {
        occupiedTiles.Add(unit.GridPosition); // track this unit 
    }

    public void UnregisterUnit(Unit unit)
    {
        occupiedTiles.Remove(unit.GridPosition); // stop tracking unit (cause it died lmao)
    }

    public void UpdateUnitPosition(Unit unit, Vector2Int oldPos, Vector2Int newPos)
    {
        // get rid of old position and add new position, not much other info is needed but may be expanded on in the future for mechanics
        occupiedTiles.Remove(oldPos); 
        occupiedTiles.Add(newPos);
    }

    public bool IsOccupied(Vector2Int pos) => occupiedTiles.Contains(pos); // checks if a tile is occupied
}
