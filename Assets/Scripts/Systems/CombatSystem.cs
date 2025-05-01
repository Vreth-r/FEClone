using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        EventSystem.TriggerEvent(context.attacker, context.defender, EffectTrigger.OnCombatStart, context); // trigger event
        CalculateBaseStats(context); // Calculate all the damage and apply all the effects
        CombatSceneManager scene = CombatSceneManager.Instance;
        scene.EnterCombatScene(attacker, defender, context);
    }

    private static void CalculateBaseStats(CombatContext context)
    {   
        // Determine STR/DEF or ARC/RES based of weapon damage
        if(context.attackerWeapon.damageType == DamageType.Physical)
        {
            context.attackPower = context.attacker.GetModifiedStat(StatType.STR);
            context.defensePower = context.defender.GetModifiedStat(StatType.DEF);
        }
        else
        {
            context.attackPower = context.attacker.GetModifiedStat(StatType.ARC);
            context.defensePower = context.defender.GetModifiedStat(StatType.RES);
        }

        // Calculate other events based off stats
        context.hitRate = context.attacker.hit;
        context.avoid = context.defender.avoidance;

        context.hitChance = Mathf.Clamp(context.hitRate - context.avoid, 0, 100);

        context.critRate = context.attacker.crit;
        context.critAvoid = context.defender.GetModifiedStat(StatType.LCK);

        context.critChance = Mathf.Clamp(context.critRate - context.critAvoid, 0, 100);
        
        // Trigger Event
        EventSystem.TriggerEvent(context.attacker, context.defender, EffectTrigger.OnHit, context);

        // Final damage that will be shaved off the defending unit preset with the base damage just from stat difference
        context.finalDamage = Mathf.Max(0, context.attackPower - context.defensePower);
        ResolveCombat(context);
    }

    private static void ResolveCombat(CombatContext context)
    {
        // determine if unit is hitting and/or critting
        context.hitting = context.attacker.Roll(context.hitChance);
        context.critting = context.attacker.Roll(context.critChance);
        Debug.Log(context.hitting);
        if (context.hitting)
        {
            if (context.critting)
            {
                context.finalDamage = Mathf.FloorToInt(context.finalDamage * 1.5f);
            }
            context.defender.TakeDamage(context.finalDamage); // unit death is handled in unit
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
            attackerWeapon = counterWeapon,
            isPlayerAttack = false
        };

        CalculateBaseStats(counterContext);
        EventSystem.TriggerEvent(context.attacker, context.defender, EffectTrigger.OnHit, context);
        ResolveCombat(counterContext);
    }

    private static bool InRange(Unit attacker, Unit target, WeaponItem weapon)
    {
        int dist = Mathf.Abs(attacker.GridPosition.x - target.GridPosition.x) +
                    Mathf.Abs(attacker.GridPosition.y - target.GridPosition.y);
        
        return dist >= weapon.minRange && dist <= weapon.maxRange;
    }
}
