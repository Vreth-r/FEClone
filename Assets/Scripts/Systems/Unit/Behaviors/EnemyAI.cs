using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System;

[RequireComponent(typeof(Unit))]
[RequireComponent(typeof(UnitMovement))]
public class EnemyAI : MonoBehaviour
{
    private Unit _unit;
    private UnitMovement _movement;
    private BehaviourNode _rootNode;
    private CancellationTokenSource _cts;

    public bool TurnComplete { get; private set; }

    private void Awake()
    {
        _cts = new CancellationTokenSource();
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
        _unit.OnDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        _unit.OnDeath -= HandleDeath;
        _cts?.Cancel();
        _cts?.Dispose();
    }

    private void HandleDeath(Unit dead)
    {
        if (!_cts.IsCancellationRequested)
        {
            _cts.Cancel();
        }
        TurnComplete = true;
    }

    public async UniTask RunTurnAsync()
    {
        TurnComplete = false;
        Debug.Log($"[EnemyAI] {_unit.unitName} starting turn...");

        try
        {
            await _rootNode.RunAsync(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning($"[EnemyAI] {_unit.unitName} turn interupt");
        }

        if (_unit == null || _unit.currentHP <= 0)
        {
            TurnComplete = true;
            return;
        }

        _unit.state = UnitState.Tapped;
        TurnComplete = true;
        Debug.Log($"[EnemyAI] {_unit.unitName} finished turn");
    }
}
