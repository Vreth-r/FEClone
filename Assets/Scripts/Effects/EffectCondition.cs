using UnityEngine;

public abstract class EffectCondition : ScriptableObject
{
    public abstract bool IsSatisfied(Unit source, Unit target, EffectContext context);
}