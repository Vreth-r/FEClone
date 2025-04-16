using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles unit movement, a complex endeavor apparently
// To be attached to each unit instance

public class UnitMovement : MonoBehaviour
{
    private Unit unit;
    private bool isSelected = false;

    private void Start()
    {
        unit = GetComponent<Unit>(); // grab unit reference on the prefab
    }

    private void OnMouseDown()
    {
        if(unit.team != Team.Player || TurnManager.Instance.currentTurn != TurnState.Player)
        {
            return; // dont select it if its not the players unit or turn
        }

        isSelected = !isSelected; // toggle selection state
        Debug.Log(isSelected ? "Unit Selected" : "Unit Deselected");
        Debug.Log($"position: {unit.GridPosition}");

        if (isSelected) // if selected, show movement range tiles
        {
            MovementRange.Instance.ShowRange(unit.GridPosition, unit.movementRange, unit.attackRange);  
        }
        else // otherwise hide them
        {
            MovementRange.Instance.ClearHighlights();
        }
    }

    private void Update()
    {
        if (!isSelected)
        {
            return; // if not selected, dont do anything
        }

        if(Input.GetMouseButtonDown(1)) // on right click
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // get mouse world coords
            Vector3Int cell = GridManager.Instance.WorldToCell(worldPos); // get the cell according to those cords
            Vector2Int gridPos = new(cell.x, cell.y);
            Vector2Int oldPos = unit.GridPosition; // get units old position b4 move

            if(!MovementRange.Instance.isMoveableTo(gridPos))
            {
                Debug.Log("Invalid Move.");
                return;
            }

            Vector3 targetWorld = GridManager.Instance.CellToWorld(cell) + new Vector3(0.5f, 0.5f, 0); // get the proper coords according to that cell, offset for centering
            transform.position = new Vector3(targetWorld.x, targetWorld.y, transform.position.z); // move the unit
            unit.GridPosition = gridPos; // update its grid position
            UnitManager.Instance.UpdateUnitPosition(unit, oldPos, gridPos); // check in with the unit manager

            isSelected = false; // turn that shit off
            MovementRange.Instance.ClearHighlights();
        }
    }
}
