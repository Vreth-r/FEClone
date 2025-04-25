using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ill comment this later cause im still developing it (also untested)
// still need to add stuff like
/*
    - Followup attacks (+5 speed than defender)
    - terrain bonuses
    - weapon type modifiers
    - animations (later way later)
*/
public class CombatSystem
{
    public static void StartCombat(Unit attacker, Unit defender)
    {
        Debug.Log($"{attacker.unitName} attacks {defender.unitName}");

        CombatContext context = new ()
        {
            isPlayerAttack = true,
            attacker = attacker,
            defender = defender,
            weapon = attacker.equippedItem as WeaponItem
        };

        if (context.weapon == null)
        {
            Debug.Log("No weapon equipped");
            return;
        }

        CalculateBaseStats(context);
        EventSystem.TriggerEvent(EffectTriggerType.OnCombatStart, context.attacker, context.defender, context);
        ResolveCombat(context);
        TryCounterattack(context);
    }

    private static void CalculateBaseStats(CombatContext context)
    {   
        // need to change this to support magical/physical so either arcane or strength
        context.attackPower = context.attacker.GetModifiedStat(context.attacker.strength, "STR");

        context.hitRate = (float)context.attacker.hit;
        context.avoid = (float)context.defender.avoidance;

        context.hitChance = Mathf.Clamp(context.hitRate - context.avoid, 0, 100);

        context.critRate = (float)context.attacker.crit;
        context.critAvoid = (float)context.defender.GetModifiedStat(context.defender.luck, "LCK");

        context.critChance = Mathf.Clamp(context.critRate - context.critAvoid, 0, 100);
        
        context.defensePower = context.weapon.weaponType.damageType == DamageType.Physical
            ? context.defender.GetModifiedStat(context.defender.defense, "DEF")
            : context.defender.GetModifiedStat(context.defender.resistance, "RES");
        
        EventSystem.TriggerEvent(EffectTriggerType.OnHit, context.attacker, context.defender, context);

        context.finalDamage = Mathf.Max(0, context.attackPower - context.defensePower);
    }

    private static void ResolveCombat(CombatContext context)
    {
        int hitRoll = Random.Range(0, 100);
        if (hitRoll < context.hitChance)
        {
            Debug.Log("Hit");

            bool isCrit = Random.Range(0, 100) < context.critChance;
            int finalDamage = context.finalDamage;
            if (isCrit)
            {
                Debug.Log("Crit");
                finalDamage *= 3;
            }

            context.defender.currentHP -= finalDamage;
            Debug.Log($"{context.attacker.unitName} dealth {finalDamage} damage to {context.defender.name}");

            if (context.defender.currentHP <= 0)
            {
                Debug.Log($"{context.defender.unitName} died");
                // handle unit death here
            }
        }
        else
        {
            Debug.Log("Miss");
        }
    }

    private static void TryCounterattack(CombatContext context)
    {
        // simple counterattack if in range
        var counterWeapon = context.defender.equippedItem as WeaponItem;
        if (counterWeapon == null) return;

        bool inRange = InRange(context.defender, context.attacker, counterWeapon);
        if (!inRange) return;

        Debug.Log($"{context.defender.name} counterattacks");

        CombatContext counterContext = new()
        {
            attacker = context.defender,
            defender = context.attacker,
            weapon = counterWeapon,
            isPlayerAttack = false
        };

        CalculateBaseStats(counterContext);
        EventSystem.TriggerEvent(EffectTriggerType.OnHit, context.attacker, context.defender, context);
        ResolveCombat(counterContext);
    }

    private static bool InRange(Unit attacker, Unit target, WeaponItem weapon)
    {
        int dist = Mathf.Abs(attacker.GridPosition.x - target.GridPosition.x) +
                    Mathf.Abs(attacker.GridPosition.y - target.GridPosition.y);
        
        return dist >= weapon.minRange && dist <= weapon.maxRange;
    }
}
