using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public abstract void Apply(Unit source, Unit target, EffectContext context);
}