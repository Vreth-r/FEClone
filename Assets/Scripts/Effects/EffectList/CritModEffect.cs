using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Critical Damage Modifier Effect:
/// Will set a units critical power stat for a combat instance.
/// Parameter scheme:
///     string Mod : float [mod]
/// </summary>
[CreateAssetMenu(menuName = "Tactics RPG/Effects/Crit Mod")]
public class CritModEffect : Effect
{
    public override void Apply(Unit source, Unit target, EffectContext context)
    {
        var p = context.parameters;
        foreach (var param in p.paramMap)
        {
            if(param.Key == "Mod")
            {
                context.combat.critPower = p.GetFloat(param.Key);
            }
        }
    }
}
