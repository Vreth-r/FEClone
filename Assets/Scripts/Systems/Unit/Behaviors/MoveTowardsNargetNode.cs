using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsTargetNode : BehaviourNode
{
    private readonly Unit _enemy;
    private readonly UnitMovement _movement;
    private readonly FindNearestPlayerNode _targetFinder;

    private bool _isMoving = false;
    private bool _hasMoved = false;

    public MoveTowardsTargetNode(Unit enemy, UnitMovement movement, FindNearestPlayerNode targetFinder)
    {
        _enemy = enemy;
        _movement = movement;
        _targetFinder = targetFinder;
    }

    public override State Evaluate()
    {
        if (_hasMoved)
        {
            _state = State.Success;
            return _state;
        }

        Unit target = _targetFinder.GetTarget();
        if (target == null)
        {
            _state = State.Failure;
            return _state;
        }

        Vector2Int start = _enemy.GridPosition;
        Vector2Int goal = target.GridPosition;

        // --- STEP 1: Find a valid destination within attack range ---
        Vector2Int destination = FindBestAttackTile(start, goal);

        if (destination == start)
        {
            // Already in range or no valid path
            _hasMoved = true;
            _state = State.Success;
            return _state;
        }

        // --- STEP 2: Start moving ---
        if (!_isMoving)
        {
            _isMoving = true;
            _enemy.StartCoroutine(MoveToDestination(destination));
        }

        if (_isMoving)
        {
            _state = State.Running;
        }
        else
        {
            _hasMoved = true;
            _state = State.Success;
        }

        return _state;
    }

    private Vector2Int FindBestAttackTile(Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> possibleTiles = new();

        // Collect all tiles within attack range of the target
        for (int x = -_enemy.attackRange; x <= _enemy.attackRange; x++)
        {
            for (int y = -_enemy.attackRange; y <= _enemy.attackRange; y++)
            {
                Vector2Int pos = goal + new Vector2Int(x, y);
                if (Vector2Int.Distance(pos, goal) <= _enemy.attackRange)
                    possibleTiles.Add(pos);
            }
        }

        Vector2Int bestTile = start;
        int bestCost = int.MaxValue;

        foreach (var tile in possibleTiles)
        {
            // Skip the tile if occupied or invalid
            //if (!GridManager.Instance.IsInsideBounds(tile))
                //continue;
            if (UnitManager.Instance.IsOccupied(tile))
                continue;
            if (GridManager.Instance.GetTerrainAt(tile).impassable)
                continue;

            // Try to find a path to this tile
            List<Vector2Int> path = Pathfinding.FindPath(start, tile, _movement.GetMovementRange().IsWalkable);
            if (path == null || path.Count == 0)
                continue;

            // Check total cost
            int cost = CalculatePathCost(path);
            if (cost < bestCost && cost <= _enemy.movementRange)
            {
                bestCost = cost;
                bestTile = tile;
            }
        }

        return bestTile;
    }

    private int CalculatePathCost(List<Vector2Int> path)
    {
        int cost = 0;
        for (int i = 1; i < path.Count; i++)
        {
            TerrainTile tile = GridManager.Instance.GetTerrainAt(path[i]);
            if (tile != null)
                cost += tile.moveCost;
        }
        return cost;
    }

    private IEnumerator MoveToDestination(Vector2Int destination)
    {
        yield return _movement.MoveTo(destination);
        _isMoving = false;
    }
}
