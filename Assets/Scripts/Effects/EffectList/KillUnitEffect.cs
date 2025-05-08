using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Effect to kill a unit, no exceptions
/*  
Parameter scheme:
    nada
*/
[CreateAssetMenu(menuName = "Tactics RPG/Effects/Kill Unit")]
public class KillUnitEffect : Effect
{
    public override void Apply(Unit source, Unit target, EffectContext context)
    {
        target.TakeDamage(target.GetStatByType(StatType.CHP)); 
        // death logic is handled through that func
    }
}
