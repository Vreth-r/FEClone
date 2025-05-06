using System.Collections;
using System.Collections.Generic;
public static class EventSystem
{
    public static void TriggerEvent(
        Unit source, // the source unit of the event
        Unit target, // the target of the unit source
        Event evnt, // what event to actually trigger
        CombatContext combatContext = null // if theres a need for combat
    ) 
    {
        EffectContext ctx = new() { combat = combatContext }; // add the context

        List<EffectInstance> allEffects = new(); // list of all effects to trigger

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
            effect.TryApply(source, target, evnt, ctx);
        }
    }
}

public enum Event
{
    // Will be adding more later just this for now while deving it out
    Passive,
    OnCombatStart,
    OnHit,
    OnCrit,
    OnKill,
    OnTurnStart,
    OnWait,
    OnMove,
    OnSkillUse,
    Custom
}