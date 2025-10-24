using Cysharp.Threading.Tasks;
using UnityEngine;

public class AttackTargetNode : BehaviourNode
{
    private readonly Unit _enemy;
    private readonly FindNearestPlayerNode _targetFinder;

    public AttackTargetNode(Unit enemy, FindNearestPlayerNode targetFinder)
    {
        _enemy = enemy;
        _targetFinder = targetFinder;
    }

    public override async UniTask<State> RunAsync()
    {
        Unit target = _targetFinder.Target;
        if (target == null)
            return State.Failure;

        float dist = GridManager.Instance.GetTileDistance(_enemy.GridPosition, target.GridPosition);
        if (dist > _enemy.attackRange)
            return State.Failure;

        await CombatSystem.StartCombat(_enemy, target);
        return State.Success;
    }
}
