using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// <c>EffectInstance</c>:
/// An effect instance is a collection of an effect (backend code), parameters, triggers, and conditions.
/// Effect instances are designed to fit into containers like Skills and Items.
/// Parameters, triggers, and conditions are defined in the effect instances' container scriptable object or json data.
/// </summary>
[System.Serializable]
public class EffectInstance
{
    public Effect effect; // logic
    public List<Parameter> parameters; // set in editor
    public List<EffectTriggerData> triggerConditions; // set in editor
    public bool selfTarget; // set in editor

    public void Apply(Unit source, Unit target, EffectContext context = null)
    {
        context.parameters = new ParameterMap(parameters); // load the parameters into the map
        if (selfTarget)
        {
            effect.Apply(source, source, context);
        }
        else
        {
            effect.Apply(source, target, context);
        }
    }
}

/// <summary>
/// <c>Parameter</c>:
/// A parameter is a collection of a string key and 3 other values of types float, bool, and string.
/// </summary>
[System.Serializable]
public class Parameter
{
    public string key;
    public ParameterType type;

    public float floatValue;
    public bool boolValue;
    public string stringValue;

    public object GetValue()
    {
        return type switch
        {
            ParameterType.Float => floatValue,
            ParameterType.Bool => boolValue,
            ParameterType.String => stringValue,
            _ => null
        };
    }
}

/// <summary>
/// <c>ParameterMap</c>:
/// A parameter map is a collection of parameters easily accessible in dicitonary form for quick access time.
/// </summary>
public class ParameterMap
{
    public readonly Dictionary<string, Parameter> paramMap = new();

    public ParameterMap(List<Parameter> parameters)
    {
        foreach (var p in parameters)
        {
            paramMap[p.key] = p;
        }
    }

    public float GetFloat(string key, float defaultValue = 0f)
    {
        if (paramMap.TryGetValue(key, out var param) && param.type == ParameterType.Float)
        {
            return param.floatValue;
        }
        return defaultValue;
    }

    public int GetInt(string key, float defaultValue = 0f)
    {
        if (paramMap.TryGetValue(key, out var param) && param.type == ParameterType.Float)
        {
            return Mathf.FloorToInt(param.floatValue);
        }
        return Mathf.FloorToInt(defaultValue);
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        if (paramMap.TryGetValue(key, out var param) && param.type == ParameterType.Bool)
        {
            return param.boolValue;
        }
        return defaultValue;
    }

    public string GetString(string key, string defaultValue = "")
    {
        if (paramMap.TryGetValue(key, out var param) && param.type == ParameterType.String)
        {
            return param.stringValue;
        }
        return defaultValue;
    }
}

/// <summary>
/// <c>EffectTriggerData</c>:
/// Effect Trigger Data defines the percentage chance for an effect instance to to activate.
/// It contains the total stat procs (which is just percent activation chance based off of stat values),
/// the base additional flat chance,
/// and any conditions that need to be fulfilled in order for the effect to even roll its procs.
/// </summary>
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


/// <summary>
/// <c>EffectCondition</c>:
/// An effect condition is a circumstance that must be met in order for an effect instance to 
/// roll its chances of activating.
/// </summary>
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

            /* implement later when backend can support it
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

// Just a double type container really
[System.Serializable]
public class StatProc
{
    public StatType stat;
    public float multiplier;
}

// another double type container lmao

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

public class EffectContext
{
    public ParameterMap parameters;
    public CombatContext combat;
    public GridManager grid;
}

public enum ParameterType
{
    Float, Bool, String
}