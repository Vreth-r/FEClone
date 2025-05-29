using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// pathfinding alg for showing movement/attack ranges
// was built on A* but that was insane and not neccessary so its modified to be Dijkstra
// you prob dont need to change this unless you know what youre doing

public class Pathfinding
{
    public class Node
    {
        public Vector2Int position;
        public int gCost; // movement so far
        public int hCost; // heuristic
        public int FCost => gCost + hCost;
        public Node parent;

        public Node(Vector2Int pos) => position = pos;
    }

    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int target, System.Func<Vector2Int, bool> isWalkable, TerrainManager terrainManager)
    {
        Dictionary<Vector2Int, Node> allNodes = new();
        PriorityQueue<Node> open = new(); // simple priority queue
        HashSet<Vector2Int> closed = new();

        Node startNode = new(start) { gCost = 0, hCost = Heuristic(start, target) };
        open.Enqueue(startNode);
        allNodes[start] = startNode;

        while (open.Count > 0)
        {
            Node current = open.Dequeue();
            if (current.position == target)
                return ReconstructPath(current);

            closed.Add(current.position);

            foreach (var dir in Directions)
            {
                Vector2Int neighborPos = current.position + dir;
                if (closed.Contains(neighborPos) || !isWalkable(neighborPos))
                    continue;
                TerrainTile terrain = terrainManager.GetTerrainAt(neighborPos);
                if (terrain == null) continue;
                int terrainCost = terrainManager.GetTerrainCost(terrain);
                int moveCost = current.gCost + terrainCost;

                if (!allNodes.TryGetValue(neighborPos, out Node neighbor))
                {
                    neighbor = new Node(neighborPos);
                    allNodes[neighborPos] = neighbor;
                }

                if (moveCost < neighbor.gCost || neighbor.gCost == 0)
                {
                    neighbor.gCost = moveCost;
                    neighbor.hCost = Heuristic(neighborPos, target);
                    neighbor.parent = current;

                    if (!open.Contains(neighbor))
                        open.Enqueue(neighbor);
                }
            }
        }

        return null; // no path
    }

    private static int Heuristic(Vector2Int a, Vector2Int b) =>
        Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    private static List<Vector2Int> ReconstructPath(Node node)
    {
        List<Vector2Int> path = new();
        while (node != null)
        {
            path.Add(node.position);
            node = node.parent;
        }
        path.Reverse();
        return path;
    }

    private static readonly List<Vector2Int> Directions = new()
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };
}

