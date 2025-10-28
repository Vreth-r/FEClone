using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

// effects proced through an action will be designed to just proc, no conditions or anything
[CreateAssetMenu(menuName = "Units/Actions/Apply Effect Action")]
public class ApplyEffectAction : UnitAction
{
    public List<EffectInstance> effects;
    public override async UniTask ExecuteAsync(Unit unit)
    {
        unit.unitMovement.GetMovementRange().ClearHighlights();
        unit.state = UnitState.Tapped;
        unit.GetComponent<UnitMovement>().enabled = false;
        foreach (var e in effects)
        {
            e.Apply(unit, unit);
        }
        TurnManager.Instance.TryEndPlayerTurn();
        await UniTask.Yield();
    }
}