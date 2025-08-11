using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Flat Bonus Damage Effect:
/// Applies extra damage onto a single strike in combat
/// Parameter scheme:
///    string Mod : int [dmg]
/// </summary>
[CreateAssetMenu(menuName = "Tactics RPG/Effects/Bonus Damage")]
public class FlatBonusDamageEffect : Effect
{
    public override void Apply(Unit source, Unit target, EffectContext context)
    {
        var p = context.parameters;
        foreach (var param in p.paramMap)
        {
            if(param.Key == "Mod")
            {
                context.combat.bonusDamage += p.GetInt(param.Key);
            }
        }
    }
}