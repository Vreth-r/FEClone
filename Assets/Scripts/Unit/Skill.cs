using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Skills are aspects of a unit that help them in combat
// ex: Sol from FE Awakening allows the unit to heal for 50% of the damage they deal from a strike, trigger chance was skill stat / 2

// In this game, skills are containers for really cool effects
[CreateAssetMenu(menuName = "Tactics RPG/Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;
    public List<EffectInstance> effects;

    public void ProcEffects(Unit source, Unit target, Event evnt, EffectContext context = null)
    {
        foreach(var e in effects)
        {
            foreach(var triggerData in e.triggerConditions)
            {
                if(triggerData.evnt != evnt) continue; // skip it if its event isnt being proced

                if(context == null) context = new EffectContext(); // create context if there isnt already

                if(!triggerData.AreConditionsMet(source, target, context)) continue; // check conditions

                if(triggerData.evnt == Event.Passive)
                {
                    // always apply passives
                    e.Apply(source, target, context);
                }
                else
                {
                    int chance = Mathf.FloorToInt(triggerData.GetProcChance(source));
                    if(source.Roll(chance))
                    {
                        Debug.Log($"Skill [{skillName}] triggered by {source.unitName}! Chance: {chance:F1}%");
                        e.Apply(source, target, context);
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class EffectTriggerData
{
    public Event evnt;

    // defines all the shit like [trigger % = SPD/2 + ARC/2]
    public List<StatProc> procs;
    public float flatChance;

    public List<EffectCondition> conditions;

    public float GetProcChance(Unit source)
    {
        float total = flatChance;
        foreach (var component in procs)
        {
            total += source.GetModifiedStat(component.stat) * component.multiplier;
        }
        return total;
    }

    public bool AreConditionsMet(Unit source, Unit target, EffectContext context = null)
    {
        foreach(var cond in conditions)
        {
            if(!cond.Evaluate(source, target, context)) return false;
        }
        return true;
    }
}


// Yes im hardcoding these because i dont expect a lot.
[System.Serializable]
public class EffectCondition
{
    public ConditionType conditionType;
    public string stringValue;
    public float floatValue;
    public bool invert; // for inverting condition (could be useful)

    public bool Evaluate(Unit source, Unit target, EffectContext context = null)
    {
        bool result = false;

        switch(conditionType)
        {
            case ConditionType.TargetHPPercentLessThan:
                float percent = (float)target.GetStatByType(StatType.CHP) / target.GetModifiedStat(StatType.MHP);
                result = percent < floatValue;
                break;

            case ConditionType.TargetHasTag:
                result = target.unitClass.classTags.Contains(target.unitClass.GetTagFromName(stringValue));
                break;

            case ConditionType.SourceHasTag:
                result = source.unitClass.classTags.Contains(source.unitClass.GetTagFromName(stringValue));
                break; 

            case ConditionType.TargetClassIs:
                result = target.unitClass.className == stringValue;
                break;

            /* implement later
            case ConditionType.TerrainTypeIs:
                if(context != null && context.combat != null && context.combat.terrain != null) result = context.combat.terrainTile.terrainType == stringValue;
                break;
            */
            
            // case ConditionType.TurnCountGreaterThan: need to touch the turnManager script

            case ConditionType.IsAttacker:
                result = context?.combat?.attacker == source;
                break;

            case ConditionType.IsDefender:
                result = context?.combat?.defender == source;
                break;
        }

        return invert ? !result : result; // flips that shit should that shit be required.
    }
}

[System.Serializable]
public class StatProc
{
    public StatType stat;
    public float multiplier;
}

// a trigger is just an event and pMap coupling with support methods
[System.Serializable]
public class Trigger
{
    public Event evnt;
    public List<Parameter> parameters;
}

public enum ConditionType
{
    TargetHPPercentLessThan,
    TargetHasTag,
    SourceHasTag,
    TargetClassIs,
    TerrainTypeIs,
    TurnCountGreaterThan,
    IsAttacker,
    IsDefender
}
