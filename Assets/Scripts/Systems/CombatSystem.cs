using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// still need to add stuff like
/*
    - terrain bonuses
    - weapon type modifiers
    - animations (later way later)
*/
public class CombatSystem
{
    private const int followUpSpeedThreshold = 5;
    public static void StartCombat(Unit attacker, Unit defender)
    // called at the start of combat and initializes everything
    {
        CombatContext context = new () // creates the combat context and initialzizes it
        {
            isPlayerAttack = true,
            attacker = attacker,
            defender = defender,
            attackerWeapon = attacker.equippedItem as WeaponItem,
            defenderWeapon = defender.equippedItem as WeaponItem
        };

        if (context.attackerWeapon == null) // if no weapon, no combat
        {
            Debug.Log("No weapon equipped");
            return;
        }
        context.attackerPrevHP = context.attacker.currentHP;
        context.defenderPrevHP = context.defender.currentHP;
        EventSystem.TriggerEvent(context.attacker, context.defender, Event.OnCombatStart, context); // trigger event
        CalculateBaseStats(context); // Calculate all the damage and apply all the effects
        CombatSceneManager scene = CombatSceneManager.Instance;
        scene.EnterCombatScene(attacker, defender, context);
    }

    private static void CalculateBaseStats(CombatContext context)
    {   
        // Determine STR/DEF or ARC/RES based of weapon damage
        context.attackPower = context.attackerWeapon.damageType == DamageType.Physical
            ? context.attacker.GetModifiedStat(StatType.STR)
            : context.attacker.GetModifiedStat(StatType.ARC);

        context.defensePower = context.attackerWeapon.damageType == DamageType.Physical
            ? context.defender.GetModifiedStat(StatType.DEF)
            : context.defender.GetModifiedStat(StatType.RES);

        // Calculate other events based off stats
        context.hitRate = context.attacker.hit;
        context.avoid = context.defender.avoidance;
        context.hitChance = Mathf.Clamp(context.hitRate - context.avoid, 0, 100);

        context.critRate = context.attacker.crit;
        context.critAvoid = Mathf.FloorToInt(context.defender.GetModifiedStat(StatType.LCK)/3);
        context.critChance = Mathf.Clamp(context.critRate - context.critAvoid, 0, 100);

        int rawDamage = context.attackPower - context.defensePower;
        context.baseDamage = Mathf.Max(0, rawDamage);
        Debug.Log($"atk - def: {context.baseDamage}");

        // first attack
        ResolveAttack(context);

        // retaliation (defender counterattack)
        if(context.defenderWeapon != null && InRange(context.defender, context.attacker, context.defenderWeapon) && context.defender.currentHP > 0)
        {
            Debug.Log($"{context.defender.unitName} counterattacks");

            var counterContext = new CombatContext
            {
                attacker = context.defender,
                defender = context.attacker,
                attackerWeapon = context.defenderWeapon,
                isPlayerAttack = false,
                isCounterAttack = true
            };

            ResolveAttack(counterContext);
        }

        // Follow-up attack (only attacker can double up if attacker spd is +5 than defender spd)
        if (context.defender.currentHP > 0 && context.attacker.GetModifiedStat(StatType.SPD) - context.defender.GetModifiedStat(StatType.SPD) >= followUpSpeedThreshold)
        {
            Debug.Log($"{context.attacker.unitName} makes a follow-up attack");

            ResolveAttack(context);
        }

        EventSystem.TriggerEvent(context.attacker, context.defender, Event.OnCombatEnd, context);
        /*
        // determine if unit is hitting and/or critting
        context.hitting = context.attacker.Roll(context.hitChance);
        context.critting = context.attacker.Roll(context.critChance);
        if (context.hitting)
        {
            // Trigger Event
            EventSystem.TriggerEvent(context.attacker, context.defender, Event.OnHit, context);
            if (context.critting)
            {
                EventSystem.TriggerEvent(context.attacker, context.defender, Event.OnCrit, context);
                context.finalDamage = Mathf.FloorToInt((context.baseDamage + context.bonusDamage) * context.critPower);
                Debug.Log($"with bonus and crit: {context.finalDamage}");
            }
            else
            {
                context.finalDamage = context.baseDamage + context.bonusDamage;
                Debug.Log($"with bonus: {context.finalDamage}");
            }
        }
        // would add an OnMiss event here should we plan for any effects for that
        context.defender.TakeDamage(context.finalDamage);
        EventSystem.TriggerEvent(context.attacker, context.defender, Event.OnCombatEnd, context); // trigger event
        */
    }

    private static void ResolveAttack(CombatContext context)
    {
        // Roll hit
        context.hitting = context.attacker.Roll(context.hitChance);
        context.critting = context.attacker.Roll(context.critChance);
        context.finalDamage = 0;
        context.bonusDamage = 0;

        if(context.hitting)
        {
            EventSystem.TriggerEvent(context.attacker, context.defender, Event.OnHit, context);
            if(context.critting)
            {
                EventSystem.TriggerEvent(context.attacker, context.defender, Event.OnCrit, context);
                context.finalDamage = Mathf.FloorToInt((context.baseDamage + context.bonusDamage) * context.critPower);
                Debug.Log("Crit");
            }
            else
            {
                context.finalDamage = context.baseDamage + context.bonusDamage;
                Debug.Log("Hit");
            }

            context.defender.TakeDamage(context.finalDamage);
            Debug.Log($"{context.attacker.unitName} deals {context.finalDamage} damage to {context.defender.unitName}");
        }
        else
        {
            Debug.Log($"{context.attacker.unitName} missed {context.defender.unitName}");
            // Optionally trigger OnMiss event here
        }
    }

    private static bool InRange(Unit attacker, Unit target, WeaponItem weapon)
    {
        int dist = Mathf.Abs(attacker.GridPosition.x - target.GridPosition.x) +
                    Mathf.Abs(attacker.GridPosition.y - target.GridPosition.y);
        
        return dist >= weapon.minRange && dist <= weapon.maxRange;
    }
}
