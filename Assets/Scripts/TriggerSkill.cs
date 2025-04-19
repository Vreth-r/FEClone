using UnityEngine;

public abstract class TriggerSkill : Skill
{
    public override void Apply(Unit unit)
    {
        return;
    }
    public abstract bool ShouldTrigger(Unit attacker, Unit defender, CombatContext context);
    public abstract void ApplyEffect(Unit attacker, Unit defender, CombatContext context);
}
