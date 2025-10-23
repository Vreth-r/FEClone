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

    public override State Evaluate()
    {
        Unit target = _targetFinder.GetTarget();
        if (target == null)
        {
            _state = State.Failure;
            return _state;
        }

        float dist = GridManager.Instance.GetTileDistance(_enemy.GridPosition, target.GridPosition);
        if (dist > _enemy.attackRange)
        {
            _state = State.Failure;
            return _state;
        }

        // hook combat logic here
        Debug.Log($"{_enemy.unitName} attacks {_targetFinder.GetTarget().unitName}!");
        CombatSystem.StartCombat(_enemy, target);

        _enemy.state = UnitState.Tapped;
        _state = State.Success;
        return _state;
    }
}
