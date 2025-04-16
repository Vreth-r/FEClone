using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// The movement preview/system in general is suprisingly complex, this class is purely for
// showing the highlights of a units movement/attack range, taking account for terrain
// costs, and impassibles

public class MovementRange : MonoBehaviour
{
    public static MovementRange Instance;

    public Tilemap highlightTilemap; // assigned in editor, tilemap for highlight tiles
    public TileBase highlightTile; // assigned in editor, tile for movement (blue)
    public TileBase attackTile; // assigned in editor, tile for attack range (red)

    private void Awake()
    {
        Instance = this; // declare this instance for external ref
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
            highlightTilemap.SetTile((Vector3Int)current, highlightTile); // highlight the tile in blue

            foreach (Vector2Int dir in Directions) // sprawl out up down left right
            {
                Vector2Int neighbor = current + dir; // grab neighbor
                int newCost = costSoFar[current] + 1; // Assume flat movement cost (1 per tile), will change later for terrain types

                if (newCost > moveRange) // skip the rest if the unit cant move anymore
                {
                    ShowAttackRange(current, attackRange);
                    continue;
                }

                if (!IsWalkable(neighbor)) // skip the rest if the tile is occupied
                    continue;
                
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
                if(highlightTilemap.GetTile((Vector3Int)pos) == null)
                {
                    highlightTilemap.SetTile((Vector3Int)pos, attackTile); // set the red tiles on empty tiles in this tilemap
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
        return highlightTilemap.GetTile((Vector3Int)pos) != null;
    }

    private bool IsWalkable(Vector2Int pos)
    {
        // checks if the tile is not occupied and hence can be "walked" through
        // add check if its inside tilemap bounds later
        return !UnitManager.Instance.IsOccupied(pos);
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
