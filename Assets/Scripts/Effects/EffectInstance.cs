using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/*
An effect instance is just a copied version of the effect with parameters and triggers
the parameters are defined in the containers scriptable object.
*/

[System.Serializable]
public class EffectInstance
{
    public Effect effect; // logic
    public List<Parameter> parameters; // set in editor
    public List<Trigger> triggers; // set in editor

    public void TryApply(Unit source, Unit target, Event evnt, EffectContext context = null)
    {
        //if (!triggers.Contains(evnt)) return;

        context.parameters = new ParameterMap(parameters);
        effect.Apply(source, target, context);
    }
}

// a trigger is just an event and pMap coupling with support methods
[System.Serializable]
public class Trigger
{
    public Event evnt;
    public List<Parameter> parameters;
}

// a parameter is used in effect subclass code to know what its modifying based on whats set in the editor
// it is also used in triggers so the skill knows what stats its triggering off of and what modifiers to add
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

// This is weird but its basically a choose your own type string to bool || string || float dictionary
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