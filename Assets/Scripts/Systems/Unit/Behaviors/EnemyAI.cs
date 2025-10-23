using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
[RequireComponent(typeof(UnitMovement))]
public class EnemyAI : MonoBehaviour
{
    private Unit _unit;
    private UnitMovement _movement;
    private BehaviourNode _rootNode;

    private bool _turnInProgress = false;
    private bool _turnDone = false;

    private void Awake()
    {
        _unit = GetComponent<Unit>();
        _movement = GetComponent<UnitMovement>();
    }

    private void Start()
    {
        // RAH THIS IS HARDCODING
        // i will be making a visual aid i think later

        var findTarget = new FindNearestPlayerNode(_unit);
        var moveToTarget = new MoveTowardsTargetNode(_unit, _movement, findTarget);
        var attackTarget = new AttackTargetNode(_unit, findTarget);

        // ai will try to attack immediately; if not possible, move then attack.
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

    public void RunTurn()
    {
        if (_turnInProgress) return;
        _turnInProgress = true;
        _turnDone = false;

        Debug.Log("Executing Turn");
        StartCoroutine(ExecuteTurn());
    }

    private System.Collections.IEnumerator ExecuteTurn()
    {
        while (true)
        {
            var result = _rootNode.Evaluate();

            if (result == BehaviourNode.State.Success || result == BehaviourNode.State.Failure)
                break;

            yield return null;
        }

        _turnDone = true;
        _turnInProgress = false;
        _unit.state = UnitState.Tapped;
    }

    public void SetTurnInProgress(bool status)
    {
        _turnInProgress = status;
    }

    public bool IsIdle() => !_turnInProgress;
    public bool IsDone() => _turnDone;
}
