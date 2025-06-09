using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Flat Heal Effect:
/// Modifies a units health by a flat amount
/// Parameter scheme:
///    string Amount : int [heal]
/// </summary>
[CreateAssetMenu(menuName = "Tactics RPG/Effects/Flat Health Mod")]
public class FlatHealthModEffect : Effect
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
