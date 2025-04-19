using UnityEngine;

public abstract class TriggerSkill : Skill
{
    public abstract bool ShouldTrigger(Unit attacker, Unit defender, CombatContext context);
    public abstract void ApplyEffect(Unit attacker, Unit defender, CombatContext context);
    public abstract override void Apply(Unit unit); // Just so the compiler stops whining
}
