using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Effect to add bonus damage to a strike
// Can only be used in combat.
/* 
Parameter scheme:
    string Mod : float [mod]
*/
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
