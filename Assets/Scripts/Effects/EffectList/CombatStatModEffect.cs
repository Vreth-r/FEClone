using UnityEngine;

// simple effect to add a statmod to a unit
// Parameter scheme: string [stat], int [modifier]

[CreateAssetMenu(menuName = "Tactics RPG/Effects/Combat Stat Mod")]
public class CombatStatModEffect : Effect
{
    public override void Apply(Unit source, Unit target, EffectContext context)
    {
        var p = context.parameters; // a EffectParameterMap
        foreach (var param in p.paramMap) // for every parameter in the map
        {
            // implement this with StatBonusSet
            // may need to modifiy statbonusset to include a temporary combat stat mod effect.
            // this is only with flat bonuses, mult is not planned and tbh i dont really want to balance for it.
        }
    }
}
