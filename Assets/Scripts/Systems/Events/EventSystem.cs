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
            skill.ProcEffects(source, target, evnt, ctx);
        }

        // need to add weapon effects later not sure how rn
        /* for ticking down turn timers
        if(evnt == Event.OnTurnEnd)
        {
            
        }*/ 
    }
}

public enum Event
{
    // Will be adding more later just this for now while deving it out
    Passive,
    OnCombatStart, OnCombatEnd,
    OnHit,
    OnCrit,
    OnKill,
    OnTurnStart, OnTurnEnd,
    OnWait,
    OnMove,
    OnSkillUse,
    Condition, // this one is used for stuff like target hp being below a threshold
    Custom
}