using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using System.Threading;

public class MoveTowardsTargetNode : BehaviourNode
{
    private readonly Unit _enemy;
    private readonly UnitMovement _movement;
    private readonly FindNearestPlayerNode _targetFinder;

    public MoveTowardsTargetNode(Unit enemy, UnitMovement movement, FindNearestPlayerNode targetFinder)
    {
        _enemy = enemy;
        _movement = movement;
        _targetFinder = targetFinder;
    }

    public override async UniTask<State> RunAsync(CancellationToken token = default)
    {
        Unit target = _targetFinder.Target;
        if (target == null)
            return State.Failure;

        Vector2Int start = _enemy.GridPosition;
        Vector2Int goal = target.GridPosition;
        Vector2Int destination = FindBestAttackTile(start, goal);

        if (destination == start)
            return State.Success;

        token.ThrowIfCancellationRequested();
        await _movement.MoveToAsync(destination);
        return State.Success;
    }

    private Vector2Int FindBestAttackTile(Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> possibleTiles = new();
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
            if (UnitManager.Instance.IsOccupied(tile)) continue;
            if (GridManager.Instance.GetTerrainAt(tile).impassable) continue;

            List<Vector2Int> path = Pathfinding.FindPath(start, tile, _movement.GetMovementRange().IsWalkable);
            if (path == null || path.Count == 0)
                continue;

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
}
