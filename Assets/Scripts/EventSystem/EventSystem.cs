using System.Collections;
using System.Collections.Generic;
public static class EventSystem
{
    public static void TriggerEvent(
        EffectTriggerType triggerType,
        Unit source,
        Unit target = null,
        CombatContext context = null
    ) 
    {
        List<Effect> effectsToTrigger = new();

        // from source unit skills
        foreach (var skill in source.learnedSkills)
        {
            foreach (var effect in skill.effects)
            {
                if (effect.triggers.Contains(triggerType))
                {
                    effectsToTrigger.Add(effect);
                }
            }
        }

        // From weapon
        if (source.equippedItem is WeaponItem weapon)
        {
            foreach (var effect in weapon.effects)
            {
                if (effect.triggers.Contains(triggerType))
                {
                    effectsToTrigger.Add(effect);
                }
            }
        }

        // run effects
        foreach (var effect in effectsToTrigger)
        {
            effect.ApplyTrigger(source, target, context);
        }
    }
}