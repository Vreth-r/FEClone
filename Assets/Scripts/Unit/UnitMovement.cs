using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles unit movement, a complex endeavor apparently
// To be attached to each unit instance

public class UnitMovement : MonoBehaviour
{
    private Unit unit; // unit reference this script is related to
    private MovementRange movementRange;
    private bool isSelected = false; // selection status
    private Vector3 positionOffset = new Vector3(0.5f, 0.5f, 0f);
    private Vector3 preMovePosition; // just in case the player wants to cancel move
    private Vector2Int preMoveGridPos; // ^
    
    // for path previewing:
    private LineRenderer pathLine; // Line renderer for the path preview line
    private List<Vector2Int> currentPath = new(); // keeps track of the cells in the path preview
    private bool isMoving = false;
    private float moveSpeed = 5f; 
    [SerializeField] private GameObject arrowPrefab; // set in editor, the arrow at the end of the path preview
    private GameObject arrowInstance; 

    private void Start()
    {
        unit = GetComponent<Unit>(); // grab unit reference on the prefab
        movementRange = GetComponent<MovementRange>();
        pathLine = gameObject.AddComponent<LineRenderer>(); // might change this in editor later
        pathLine.positionCount = 0;
        pathLine.material = new Material(Shader.Find("Sprites/Default")); // to be changed l8r prob i dunno it looks decent enough
        pathLine.widthMultiplier = 0.1f;
        pathLine.startColor = pathLine.endColor = Color.cyan;
        if(arrowPrefab != null)
        {
            arrowInstance = Instantiate(arrowPrefab, transform); // declare instance for ref, creates ref-able game object
            arrowInstance.SetActive(false); // set that to off so its not on screen
        }
    }

    private void OnEnable()
    {
        ControlsManager.Instance.OnSelect += HandleSelect;
    }

    private void OnDisable()
    {
        ControlsManager.Instance.OnSelect -= HandleSelect;
    }

    public void SelectUnit()
    {
        if (unit.team != Team.Player || TurnManager.Instance.currentTurn != TurnState.Player || (UnitManager.Instance.isAUnitSelected() && !UnitManager.Instance.isUnitSelected(unit))
        || UIManager.Instance.GetCurrentMenuType() == MenuType.ActionMenu)
        {
            return; // you cant click on it if its not the player's unit OR turn OR if another unit is selected OR if the action menu is open (holy logic)
        }

        isSelected = !isSelected; // toggle selection state

        Debug.Log(isSelected ? "Unit Selected" : "Unit Deselected");
        Debug.Log($"position: {unit.GridPosition}");

        if (isSelected) // if selected
        {
            UnitManager.Instance.selectUnit(unit); // tell the unit manager the unit is selected
            movementRange.ShowRange(unit.GridPosition, unit.movementRange, unit.attackRange); // Show move/attack range preview tiles


        }
        else // otherwise hide them
        {
            UnitManager.Instance.deselectedUnit(); // tell the unit manager to wipe its selected unit
            movementRange.ClearHighlights(); // clear the range preview
        }
    }

    private void Update()
    {
        if (!isSelected)
        {
            return; // if not selected, dont do anything
        }

        if (!isMoving) // if not moving
        {
            Vector3Int cell = CursorController.Instance.GetCursorGridPosition(); // grabs the cell of the cursor
            Vector2Int targetPos = new(cell.x, cell.y); // set that as the target

            if (movementRange.isMoveableTo(targetPos)) // if the target cell is blue and its not the selected units space
            {
                currentPath = Pathfinding.FindPath(unit.GridPosition, targetPos, movementRange.isMoveableTo, TerrainManager.Instance); // find a path between the unit and the target thats walkable
                if (currentPath != null) DrawPath(currentPath); // if a path is found, draw it
            }
            else
            {
                pathLine.positionCount = 0; // otherwise, clear the line
                if (arrowInstance != null) arrowInstance.SetActive(false); // set the arrow to invisible
            }
        }
    }

    public void HandleSelect()
    {
        if (!isSelected) return;

        if (ControlsManager.Instance.CurrentContext != InputContext.Gameplay) return;

        if (currentPath != null && currentPath.Count > 0)
        {
            Vector3Int cell = CursorController.Instance.GetCursorGridPosition(); // grabs the cell of the cursor
            Vector2Int gridPos = new(cell.x, cell.y); // set grid pos based off cell
            if (!movementRange.isMoveableTo(gridPos)) // check if its moveable to
            {
                Debug.Log("Invalid Move.");
                return; // return if not
            }

            preMovePosition = transform.position; // cache pre move coords for cancelling
            preMoveGridPos = unit.GridPosition; // ^

            if (currentPath.Count > 1) // only walk the path if going to a new place (otherwise it just reruns previous walk)
            {
                StartCoroutine(MoveAlongPath(currentPath)); // Sets the unit to move along the path set smoothly in a co-routine yeah bro we use co-routines get used to it
            }
            else
            { // add the menu to the screen even if you dont move
                Vector3 menuWorldPos = transform.position + new Vector3(0, 0.5f, 0); // get a good pos for the menu
                UIManager.Instance.OpenMenu(MenuType.ActionMenu, this, menuWorldPos);
            }

            isSelected = false; // TURN THAT SHIT OFF CUH
            currentPath = null;
            UnitManager.Instance.deselectedUnit(); // tell the unit manager whats up
            pathLine.positionCount = 0; // reset the line renderer
            if (arrowInstance != null) arrowInstance.SetActive(false); // set the arrow to invisible
            movementRange.ClearHighlights(); // clear all the tiles
        }
    }

    private void DrawPath(List<Vector2Int> path)
    {
        // Draws a path line based of a given path 
        pathLine.positionCount = path.Count; // sets the number of verticies in the linerenderer based of the found path

        for (int i = 0; i < path.Count; i++) // for every cell in the path
        {
            Vector3 worldPos = GridManager.Instance.CellToWorld((Vector3Int)path[i]); // get its world pos
            pathLine.SetPosition(i, new Vector3(worldPos.x + positionOffset.x, worldPos.y + positionOffset.y, transform.position.z - 0.1f)); // draw the line with offset, in front of everything (z axis)
        }

        if (arrowInstance != null && path.Count > 1) // if the arrow exists (insurance, it will usually) and the path has at least a line
        {
            Vector2Int last = path[^1]; // last cell
            Vector2Int beforeLast = path[^2]; // second last cell

            Vector3 lastWorld = GridManager.Instance.CellToWorld((Vector3Int)last) + positionOffset; // grab the last cell world pos with offset
            Vector3 beforeWorld = GridManager.Instance.CellToWorld((Vector3Int)beforeLast) + positionOffset; // ^ second last cell
            Vector3 dir = (lastWorld - beforeWorld).normalized; // get the direction of the overall movement

            arrowInstance.transform.position = lastWorld; // set the arrow position
            arrowInstance.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir); // rotate it in the overall movement dir
            arrowInstance.SetActive(true); // make it visible
        }
        else if (arrowInstance != null)
        {
            arrowInstance.SetActive(false); // make it invisible if no path
        }
    }

    private IEnumerator MoveAlongPath(List<Vector2Int> path)
    {
        // Moves the unit smoothly along a given path
        isMoving = true; // set flag so update() doesnt shit itself
        Vector2Int oldPos = unit.GridPosition; // keep track of the old position

        for (int i = 1; i < path.Count; i++) // for every cell in the path
        {
            Vector3Int cell = (Vector3Int)path[i]; // ref it so were not list accessing a ton (good performance)
            Vector3 targetWorld = GridManager.Instance.CellToWorld(cell) + positionOffset; // get its world pos with offset

            while ((transform.position - targetWorld).sqrMagnitude > 0) // while the length of the vec betwix the unit position and the target is non zero
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorld, moveSpeed * Time.deltaTime); // move the unit according to movespeed and time
                yield return null; // insane backend shit but basically: wait for the next frame and continue execution from this line to give control back to editor
            }

            unit.GridPosition = path[i]; // sets its grid position for the interim
        }

        UnitManager.Instance.UpdateUnitPosition(unit, oldPos, unit.GridPosition); // tell the unit manager whats going on
        isMoving = false; // set the flag once its done to do it all over again
        if (arrowInstance != null) arrowInstance.SetActive(false);
        // jesus christ thomas yield return StartCoroutine(GameObject.Find("Main Camera").GetComponent<CameraPanner>().PanToLocation(transform.position)); // bruh hahahahahaha
        Vector3 menuWorldPos = transform.position + new Vector3(0, 0.5f, 0); // get a good pos for the menu
        UIManager.Instance.OpenMenu(MenuType.ActionMenu, this, menuWorldPos);
    }
    public void OnMenuSelect(UnitActionType action)
    // this is only here cause a lot of these actions need refs already in this file and it would be work and a half to pass
    // all the params
    {
        switch(action)
        {
            case UnitActionType.Attack:
                TargetSelector.Instance.BeginTargeting(unit);
                break; // still thinking of what to put here

            case UnitActionType.Wait:
                Debug.Log("Unit waits.");
                // TurnManager.Instance.EndTurn(); // fo l8r
                break;

            case UnitActionType.Item:
                Debug.Log("Show item UI (WIP).");
                break;
            
            case UnitActionType.Cancel:
                Debug.Log("Cancelling move in unitmovement.");
                UnitManager.Instance.UpdateUnitPosition(unit, unit.GridPosition, preMoveGridPos); // tell unit manager whats up
                transform.position = preMovePosition; // return to pre move coords
                unit.GridPosition = preMoveGridPos; // ^
                movementRange.ShowRange(unit.GridPosition, unit.movementRange, unit.attackRange); // show the movement range again
                isSelected = true; // select that shit
                UnitManager.Instance.selectUnit(unit); // tell the unit manager the unit is selected
                // we are NOT animating the move back lmao
                break;
        }
    }
}
