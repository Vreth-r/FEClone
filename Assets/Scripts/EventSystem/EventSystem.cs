using System.Collections;
using System.Collections.Generic;
public static class EventSystem
{
    public static void TriggerEvent(
        Unit source,
        Unit target,
        EffectTrigger triggerType,
        CombatContext combatContext = null
    ) 
    {
        EffectContext ctx = new() { combat = combatContext };

        List<EffectInstance> allEffects = new();

        // from source unit skills
        foreach (var skill in source.skills)
        {
            allEffects.AddRange(skill.effects);
        }

        // From weapon
        if (source.equippedItem is WeaponItem weapon)
        {
            allEffects.AddRange(weapon.effects);
        }

        // run effects
        foreach (var effect in allEffects)
        {
            effect.TryApply(source, target, triggerType, ctx);
        }
    }
}