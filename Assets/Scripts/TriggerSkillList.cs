using UnityEngine;
// Breaking a coding rule here lmao

[CreateAssetMenu(menuName = "Tactics RPG/Skill/Luna")]
public class LunaSkill : TriggerSkill
{
    // ignores defenders defense when proced
    // Trigger is 10% + Units skill + buffs /2
    [Range(0, 100)] public int baseTriggerChance;

    public override void Apply(Unit unit) // just so the compiler stops whining
    {
        return;
    }

    public override bool ShouldTrigger(Unit attacker, Unit defender, CombatContext context)
    {
        int chance = baseTriggerChance + (attacker.GetModifiedStat(attacker.skill, "SKL") / 2);
        return Random.Range(0, 100) < chance;
    }

    public override void ApplyEffect(Unit attacker, Unit defender, CombatContext context)
    {
        Debug.Log("Trigger Skill Luna activated");
        context.defensePower = 0;
        context.triggeredSkill = true;
    }
}

[CreateAssetMenu(menuName = "Tactics RPG/Skill/Wrath")]
public class WrathSkill : TriggerSkill
{
    // gives +20 crit when under 50% hp
    // Trigger is 50% HP
    public int bonusCrit = 20;

    public override void Apply(Unit unit)  // just so the compiler stops whining
    {
        return;
    }

    public override bool ShouldTrigger(Unit attacker, Unit defender, CombatContext context)
    {
        return attacker.currentHP < attacker.maxHP / 2;
    } 

    public override void ApplyEffect(Unit attacker, Unit defender, CombatContext context)
    {
        Debug.Log("Trigger Skill Wrath activated");
        // add bonusCrit to context crit when i implement it
    }
}
