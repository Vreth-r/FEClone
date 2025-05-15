using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Effect to heal a target for a flat amount of HP
/* 
Parameter scheme:
    string Amount : int [heal]
*/
[CreateAssetMenu(menuName = "Tactics RPG/Effects/Flat Heal")]
public class FlatHealEffect : Effect
{
    public override void Apply(Unit source, Unit target, EffectContext context)
    {
        var p = context.parameters;
        foreach (var param in p.paramMap)
        {
            if(param.Key == "Amount")
            {
                target.Heal(p.GetInt(param.Key));
            }
        }
    }
}
