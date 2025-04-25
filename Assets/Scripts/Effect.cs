using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public string effectName; // more of a description
    [TextArea] public string description;

    public List<EffectTriggerType> triggers;

    // called when applied to a unit (battle start or as a passive stat boost)
    public virtual void ApplyPassive(Unit unit){}

    // called during combat instances
    public virtual void ApplyTrigger(Unit source, Unit target, CombatContext context) {}

}

public enum EffectTriggerType
{
    OnTurnStart, OnTurnEnd,
    OnMove,
    OnCombatStart, OnCombatEnd,
    OnAttack, OnHit, OnKill,
    OnSkillUse,
    OnHeal,
    OnEquip,
    OnCrit,
    Custom // for scripting only events
}
