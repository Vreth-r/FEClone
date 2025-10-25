using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using System.Threading;

public class FindNearestPlayerNode : BehaviourNode
{
    private readonly Unit _enemy;
    public Unit Target { get; private set; }

    public FindNearestPlayerNode(Unit enemy)
    {
        _enemy = enemy;
    }

    public override UniTask<State> RunAsync(CancellationToken token = default)
    {
        float minDist = float.MaxValue;
        Target = null;

        foreach (var entry in UnitManager.Instance.playerUnits)
        {
            float dist = Vector2Int.Distance(_enemy.GridPosition, entry.Value.GridPosition);
            if (dist < minDist)
            {
                minDist = dist;
                Target = entry.Value;
            }
        }

        CurrentState = Target != null ? State.Success : State.Failure;
        return UniTask.FromResult(CurrentState);
    }
}
