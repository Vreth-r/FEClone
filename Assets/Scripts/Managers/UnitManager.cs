using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// purpose is to track occupied tiles so you dont have to reference 30 different units
public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private readonly Dictionary<Vector2Int, Unit> unitPositions = new(); // keeps track of occupied tiles by all units
    public Dictionary<int, Unit> playerUnits = new Dictionary<int, Unit>(); // record of all alive player units (not positions)
    public Dictionary<int, Unit> enemyUnits = new Dictionary<int, Unit>(); // record of all alive enemy units (not positions)

    public Unit selectedUnit; // for use in keeping track what unit is selected so others cant be selected at the same time
    public StatsMenu statsUI; // unit stat previewer
    public GameObject EmotePrefab; // Speech bubble w/ text prefab

    public int nextID = 11; // the reserved ID's end with Peril's ID of 10

    private void Awake() => Instance = this; // declare this instance for external ref

    public void RegisterUnit(Unit unit)
    {
        Debug.Log($"Registering {unit.unitName} at {unit.GridPosition}");
        unitPositions[unit.GridPosition] = unit; // track this unit 
        if (unit.team == Team.Player)
        {
            playerUnits[unit.unitID] = unit;
        }
        else if (unit.team == Team.Enemy) //there might be more teams later if we do npc's but i doubt it
        {
            // assign ID
            if (unit.unitID == 0)
            {
                unit.unitID = nextID;
                nextID++;
            }
            enemyUnits[unit.unitID] = unit;
        }
    }

    public void UnregisterUnit(Unit unit)
    {
        Debug.Log($"Unregistering {unit.unitName}");
        unitPositions.Remove(unit.GridPosition); // stop tracking unit (cause it died lmao)
        if (unit.team == Team.Player)
        {
            playerUnits.Remove(unit.unitID);
        }
        else if (unit.team == Team.Enemy)
        {
            enemyUnits.Remove(unit.unitID);
        }
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

    public void ClearAllUnits()
    {
        unitPositions.Clear();
    }

    // why are these four methods lowercase?
    public void selectUnit(Unit unit)
    {
        selectedUnit = unit; // selected a unit
        UIManager.Instance.OpenMenu(MenuType.StatMenu, unit);
    }

    public void deselectedUnit()
    {
        selectedUnit = null; // voids selected unit
        UIManager.Instance.CloseMenu(MenuType.StatMenu);
    }

    public bool isUnitSelected(Unit unit)
    {
        return selectedUnit == unit; // returns if the provided unit is selected
    }

    public bool isAUnitSelected()
    {
        return selectedUnit != null; // returns if there is a selected unit
    }

    public Vector2Int GetUnitPositionByName(string unitName)
    {
        foreach (Vector2Int pos in GetAllOccupiedPositions())
        {
            unitPositions.TryGetValue(pos, out Unit unit);
            if (unit.unitName == unitName)
            {
                return pos;
            }
        }
        return new Vector2Int(0, 0);
    }

    public Unit FindUnitByName(string unitName)
    {
        foreach (Unit unit in GetAllUnits()) // loops through all units, def a better way to do this
        {
            if (unit.unitName == unitName)
            {
                return unit;
            }
        }
        return null;
    }

    // for cutscenes
    public IEnumerator JumpUnit(string unitName, float numJumps)
    {
        Unit unit = FindUnitByName(unitName);
        if (unit) yield return StartCoroutine(unit.Jump(numJumps));
        yield break;
    }
    public IEnumerator MoveUnitTo(string unitName, Vector2Int gridPos)
    {
        Unit unit = FindUnitByName(unitName);
        if (unit) yield return unit.MoveTo(gridPos);
        yield break;
    }
    public IEnumerator EmoteUnit(string unitName, string emote, float duration)
    {
        Unit unit = FindUnitByName(unitName);
        if (unit) yield return unit.Emote(EmotePrefab, emote, duration);
        yield break;
    }
}
