using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// waiting to document this one i still have to test and tweak

public class TargetSelector : MonoBehaviour
{
    public static TargetSelector Instance;

    private Unit attacker;
    private List<Unit> validTargets = new();
    private bool targeting = false;

    private void Awake()
    {
        Instance = this; // declare instance for external ref
    }

    public void BeginTargeting(Unit unit)
    {
        attacker = unit;
        validTargets.Clear();
        
        WeaponItem weapon = attacker.equippedItem as WeaponItem;
        if(weapon == null)
        {
            Debug.Log("No weapon equipped");
            return;
        }

        foreach (var pos in UnitManager.Instance.GetAllOccupiedPositions())
        {
            Unit other = UnitManager.Instance.GetUnitAt(pos);
            if(other == null || other.team != Team.Enemy) continue;

            int dist = Mathf.Abs(unit.GridPosition.x - pos.x) + Mathf.Abs(unit.GridPosition.y - pos.y);
            if (dist >= weapon.minRange && dist <= weapon.maxRange)
            {
                validTargets.Add(other);
                // add extra highlighting here maybe like a brighter red tile
            }
        }

        if (validTargets.Count == 0)
        {
            Debug.Log("No enemies in range.");
            attacker = null;
            return;
        }

        Debug.Log("Select an enemy to attack.");
        targeting = true; // enable targeting mode
    }

    private void Update()
    {
        if(!targeting) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = GridManager.Instance.WorldToCell(world);
            Vector2Int gridPos = new(cell.x, cell.y);

            foreach (var target in validTargets)
            {
                if(target.GridPosition == gridPos)
                {
                    CombatSystem.StartCombat(attacker, target);
                    Clear();
                    return;
                }
            }

            Debug.Log("Invalid target"); // if clicked outside, cancel the targeting
        }
    }

    public void Clear()
    {
        // i may have other plans for this method later
        //MovementRange.Instance.ClearHighlights();
        attacker.GetComponent<MovementRange>().ClearHighlights();
        attacker = null;
        validTargets.Clear();
        targeting = false;
    }
}
