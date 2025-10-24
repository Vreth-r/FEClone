using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Unit))]
[RequireComponent(typeof(UnitMovement))]
public class EnemyAI : MonoBehaviour
{
    private Unit _unit;
    private UnitMovement _movement;
    private BehaviourNode _rootNode;

    public bool TurnComplete { get; private set; }

    private void Awake()
    {
        _unit = GetComponent<Unit>();
        _movement = GetComponent<UnitMovement>();

        // --- Build the behaviour tree (predefined list of nodes) ---
        var findTarget = new FindNearestPlayerNode(_unit);
        var moveToTarget = new MoveTowardsTargetNode(_unit, _movement, findTarget);
        var attackTarget = new AttackTargetNode(_unit, findTarget);

        _rootNode = new Sequence(new List<BehaviourNode>
        {
            findTarget,
            new Selector(new List<BehaviourNode>
            {
                attackTarget,
                new Sequence(new List<BehaviourNode> { moveToTarget, attackTarget })
            })
        });
    }

    public async UniTask RunTurnAsync()
    {
        TurnComplete = false;
        Debug.Log($"[EnemyAI] {_unit.unitName} starting turn...");

        await _rootNode.RunAsync();

        _unit.state = UnitState.Tapped;
        TurnComplete = true;

        Debug.Log($"[EnemyAI] {_unit.unitName} finished turn.");
    }
}
