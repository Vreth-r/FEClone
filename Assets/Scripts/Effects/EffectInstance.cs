using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class EffectInstance
{
    public Effect effect; // logic
    public List<EffectParameter> parameters;
    public List<EffectCondition> conditions;
    public List<EffectTrigger> triggerTypes;

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
    public int value;
}

public class EffectParameterMap
{
    private readonly Dictionary<string, int> paramMap = new();

    public EffectParameterMap(List<EffectParameter> parameters)
    {
        foreach (var p in parameters)
        {
            paramMap[p.key] = p.value;
        }
    }

    public int Get(string key, int defaultValue = 0)
    {
        return paramMap.TryGetValue(key, out int val) ? val : defaultValue;
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