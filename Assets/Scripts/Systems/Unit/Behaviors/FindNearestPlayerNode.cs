// node as in a "find nearest player" node.
using UnityEngine;

public class FindNearestPlayerNode : BehaviourNode
{
    private readonly Unit _enemy;
    private Unit _target;

    public FindNearestPlayerNode(Unit enemy)
    {
        _enemy = enemy;
    }

    public Unit GetTarget() => _target;

    public override State Evaluate()
    {
        float minDist = float.MaxValue;
        Unit nearest = null;

        foreach (var player in UnitManager.Instance.playerUnits)
        {
            // basic alg based on world distance for now, will work in tile weights later
            float dist = Vector2Int.Distance(_enemy.GridPosition, player.GridPosition);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = player;
            }
        }

        if (nearest == null)
        {
            _state = State.Failure;
            return _state;
        }

        _target = nearest;
        _state = State.Success;
        return _state;
    }
}