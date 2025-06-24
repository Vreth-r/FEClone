using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// The movement preview/system in general is suprisingly complex, this class is purely for
// showing the highlights of a units movement/attack range, taking account for terrain
// costs, and impassibles

public class MovementRange : MonoBehaviour
{
    // public static MovementRange Instance;
    private Unit unit;

    public Tilemap highlightTilemap; // assigned in editor, tilemap for highlight tiles
    public TileBase movementTile; // assigned in editor, tile for movement (blue)
    public TileBase attackTile; // assigned in editor, tile for attack range (red)

    private void Start()
    {
        
        //Instance = this; // declare this instance for external ref
        unit = GetComponent<Unit>(); // grab unit reference on the prefab
    }

    // shows movement range (blue tiles) given a start and a range
    public void ShowRange(Vector2Int origin, int moveRange, int attackRange)
    {
        ClearHighlights(); // wipes the current highlight tiles in range
        Dictionary<Vector2Int, int> costSoFar = new(); // dictionary to keep track of what tile takes what cost of movement
        Queue<Vector2Int> frontier = new(); // queue for tiles

        frontier.Enqueue(origin);
        costSoFar[origin] = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue(); // grab current tile (duh)
            if (IsWalkable(current) || current == origin)
            {
                highlightTilemap.SetTile((Vector3Int)current, movementTile); // highlight the tile in blue, anything past the origin will be checked for correctness when fed to the queue
            }
            else
            {
                highlightTilemap.SetTile((Vector3Int)current, attackTile);
            }

            foreach (Vector2Int dir in Directions) // sprawl out up down left right
            {
                Vector2Int neighbor = current + dir; // grab neighbor
                // Terrain stuff below
                TerrainTile terrain = TerrainManager.Instance.GetTerrainAt(neighbor);
                if (terrain == null || terrain.impassable) continue; // if terrain is not there or impassable skip it
                if (terrain.blocksArmored && unit.HasTag(ClassTag.Armored)) continue; // will change this logic later to just cover them all
                int moveCost = unit.HasTag(ClassTag.Flying) && terrain.ignoreForFlying // exception for flying units
                    ? 1
                    : terrain.moveCost;

                int newCost = costSoFar[current] + moveCost; // Sum up the movement costs to get to that tile

                if (newCost > moveRange || !IsWalkable(neighbor)) // skip the rest if the unit cant move anymore or if its neighbor is occupied
                {
                    ShowAttackRange(current, attackRange);
                    continue;
                }

                // if the neighbor isnt tracked yet 
                // OR 
                // its already tracked and this path takes less movement than another path already tried
                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost; // add that shit and assign its cost
                    frontier.Enqueue(neighbor); // queue up the neighbor to do this all over again
                }
            }
        }
        // This ends when it hits tiles that are either not walkable or are out of movement range
        // which is when the queue runs out of tiles
    }

    // shows attack range (red tiles) given a start and an attack range
    public void ShowAttackRange(Vector2Int origin, int attackRange)
    {
        foreach(var dir in Directions) // sprawl out
        {
            for (int i = 1; i <= attackRange; i++) // for every tile of attack range
            {
                Vector2Int pos = origin + dir * i; // get attack range for that direction
                if(!isHighlighted(pos) && TerrainManager.Instance.GetTerrainAt(pos) != null)
                {
                    highlightTilemap.SetTile((Vector3Int)pos, attackTile); // set the red tiles on terrained empty tiles in this tilemap
                }
            }
        }
    }

    public void ClearHighlights()
    {
        highlightTilemap.ClearAllTiles();
    }

    public bool isHighlighted(Vector2Int pos)
    {
        return highlightTilemap.GetTile((Vector3Int)pos) != null; // returns if tile is populated at all
    }

    public bool isMoveableTo(Vector2Int pos)
    {
        return highlightTilemap.GetTile((Vector3Int)pos) == movementTile && IsWalkable(pos); // returns if tile is blue and not occupied
    }

    public bool isAttackable(Vector2Int pos)
    {
        return highlightTilemap.GetTile((Vector3Int)pos) == movementTile || attackTile && !IsWalkable(pos); // returns if tile is blue or red and theres a unit in it
        // Semantics may change later to account for difference between units team and the enemy
    }

    public bool IsWalkable(Vector2Int pos)
    {
        // checks if the tile is not occupied and hence can be "walked" through
        // add check if its inside tilemap bounds later (maybe)
        // print(unit.unitName);
        return !UnitManager.Instance.IsOccupied(pos) || pos == unit.GridPosition;
    }

    private int Distance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // gets distance between two 2d vecs
    }

    private static readonly List<Vector2Int> Directions = new() // list of directions for use in the range algorithms
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
}
