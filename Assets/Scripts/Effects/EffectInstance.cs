using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class EffectInstance
{
    public Effect effect; // logic
    public List<EffectParameter> parameters; // set in editor
    public List<EffectCondition> conditions; // set in editor
    public List<EffectTrigger> triggerTypes; // set in editor

    public void TryApply(Unit source, Unit target, EffectTrigger currentTrigger, EffectContext context = null)
    {
        if (!triggerTypes.Contains(currentTrigger)) return;

        foreach (var condition in conditions)
        {
            if(!condition.IsSatisfied(source, target, context)) return;
        }

        var paramMap = new EffectParameterMap(parameters);
        context.parameters = paramMap;
        effect.Apply(source, target, context);
    }
}

[System.Serializable]
public class EffectParameter
{
    public string key;
    public EffectParameterType type;

    public float floatValue;
    public bool boolValue;
    public string stringValue;
    
    public object GetValue()
    {
        return type switch
        {
            EffectParameterType.Float => floatValue,
            EffectParameterType.Bool => boolValue,
            EffectParameterType.String => stringValue,
            _ => null
        };
    }
}

public class EffectParameterMap
{
    public readonly Dictionary<string, EffectParameter> paramMap = new();

    public EffectParameterMap(List<EffectParameter> parameters)
    {
        foreach (var p in parameters)
        {
            paramMap[p.key] = p;
        }
    }

    public float GetFloat(string key, float defaultValue = 0f)
    {
        if (paramMap.TryGetValue(key, out var param) && param.type == EffectParameterType.Float)
        {
            return param.floatValue;
        }
        return defaultValue;
    }

    public int GetInt(string key, float defaultValue = 0f)
    {
        if (paramMap.TryGetValue(key, out var param) && param.type == EffectParameterType.Float)
        {
            return Mathf.FloorToInt(param.floatValue);
        }
        return Mathf.FloorToInt(defaultValue);
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        if (paramMap.TryGetValue(key, out var param) && param.type == EffectParameterType.Bool)
        {
            return param.boolValue;
        }
        return defaultValue;
    }

    public string GetString(string key, string defaultValue = "")
    {
        if (paramMap.TryGetValue(key, out var param) && param.type == EffectParameterType.String)
        {
            return param.stringValue;
        }
        return defaultValue;
    }
}

public class EffectContext
{
    public EffectParameterMap parameters;
    public CombatContext combat;
    public GridManager grid;
}

public enum EffectTrigger
{
    // Will be adding more later just this for now while deving it out
    Passive,
    OnCombatStart,
    OnHit,
    OnKill,
    OnTurnStart,
    OnWait,
    OnMove,
    OnSkillUse,
    Custom
}

public enum EffectParameterType
{
    Float, Bool, String
}