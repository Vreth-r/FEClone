using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PLACEHOLDER WILL BE REPLACED/UPGRADED
public class CombatSystem
{
    public static void Attack(Unit attacker, Unit defender)
    {
        int damage = Mathf.Max(0, attacker.strength - defender.defense);
        defender.currentHP -= damage;
        Debug.Log($"{attacker.name} attacked {defender.name} for {damage} damage");
    }
    /*
    public static int CalculateDamage(Unit attacker, Unit defender, bool isPlayerAttack)
{
    CombatContext context = new()
    {
        isPlayerAttack = isPlayerAttack,
        attackPower = attacker.GetModifiedStat(attacker.strength, "STR") + (attacker.equippedItem as WeaponItem)?.might ?? 0,
        defensePower = defender.GetModifiedStat(defender.defense, "DEF"),
    };

    // Trigger attacker skills
    foreach (Skill skill in attacker.skills)
    {
        if (skill is TriggerSkill triggerSkill && triggerSkill.ShouldTrigger(attacker, defender, context))
        {
            triggerSkill.ApplyEffect(attacker, defender, context);
        }
    }

    // You can also check defender skills here for counter triggers like Vantage

    context.finalDamage = Mathf.Max(0, context.attackPower - context.defensePower);
    return context.finalDamage;
}
*/
}
