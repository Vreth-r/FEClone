using Cysharp.Threading.Tasks;
using UnityEngine;

public class FindNearestPlayerNode : BehaviourNode
{
    private readonly Unit _enemy;
    public Unit Target { get; private set; }

    public FindNearestPlayerNode(Unit enemy)
    {
        _enemy = enemy;
    }

    public override UniTask<State> RunAsync()
    {
        float minDist = float.MaxValue;
        Target = null;

        foreach (var player in UnitManager.Instance.playerUnits)
        {
            float dist = Vector2Int.Distance(_enemy.GridPosition, player.GridPosition);
            if (dist < minDist)
            {
                minDist = dist;
                Target = player;
            }
        }

        CurrentState = Target != null ? State.Success : State.Failure;
        return UniTask.FromResult(CurrentState);
    }
}
