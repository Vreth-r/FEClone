using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kill Unit Effect:
/// Kills target unit regardless of anything.
/// No Parameters.
/// </summary>
[CreateAssetMenu(menuName = "Tactics RPG/Effects/Kill Unit")]
public class KillUnitEffect : Effect
{
    public override void Apply(Unit source, Unit target, EffectContext context)
    {
        target.TakeDamage(target.GetStatByType(StatType.CHP)); 
        // death logic is handled through that method
    }
}
