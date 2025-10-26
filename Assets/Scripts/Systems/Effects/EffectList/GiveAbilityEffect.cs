using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Give Ability Effect:
/// Gives a unit the use of a specified action
/// Parameter scheme:
///    context.action
/// </summary>
[CreateAssetMenu(menuName = "Tactics RPG/Effects/Give Ability Effect")]
public class GiveAbilityEffect : Effect
{
    public override void Apply(Unit source, Unit target, EffectContext context)
    {
        Debug.Log($"Action: {context.action}");
        if (context.action != null)
        {
            target.actions.Add(context.action);
        }
    }
}