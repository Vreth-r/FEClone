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

        EventSystem.TriggerEvent(context.attacker, context.defender, Event.OnCombatStart, context); // trigger event
        CalculateBaseStats(context); // Calculate all the damage and apply all the effects
        CombatSceneManager scene = CombatSceneManager.Instance;
        scene.EnterCombatScene(attacker, defender, context);
    }

    private static void CalculateBaseStats(CombatContext context)
    {   
        context.attackerPrevHP = context.attacker.currentHP;
        context.defenderPrevHP = context.defender.currentHP;
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

        // Final damage that will be shaved off the defending unit preset with the base damage just from stat difference
        if(context.attackPower - context.defensePower < 0)
        {
            context.finalDamage = 0;
        } else 
        {
            context.finalDamage = context.attackPower - context.defensePower;
        }

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
                context.finalDamage = Mathf.FloorToInt((context.finalDamage + context.bonusDamage) * context.critPower);
            }
            else
            {
                context.finalDamage = context.finalDamage + context.bonusDamage;
            }
        }
        // would add an OnMiss event here should we plan for any effects for that
        context.defender.TakeDamage(context.finalDamage);
    }

    private static bool InRange(Unit attacker, Unit target, WeaponItem weapon)
    {
        int dist = Mathf.Abs(attacker.GridPosition.x - target.GridPosition.x) +
                    Mathf.Abs(attacker.GridPosition.y - target.GridPosition.y);
        
        return dist >= weapon.minRange && dist <= weapon.maxRange;
    }
}
