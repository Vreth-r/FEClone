using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// still need to add stuff like
/*
    - terrain bonuses
    - weapon type modifiers
    - animations (later way later)
*/

// Resolve attack is called in the combat scene manager
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

        context.isFollowingUp = context.attacker.GetModifiedStat(StatType.SPD) - context.defender.GetModifiedStat(StatType.SPD) >= followUpSpeedThreshold;

        EventSystem.TriggerEvent(context.attacker, context.defender, Event.OnCombatStart, context); // trigger event

        var queue = BuildCombatQueue(context);
        CombatSceneManager.Instance.EnterCombatScene(attacker, defender, context, queue);
    }

    public static CombatQueue BuildCombatQueue(CombatContext context)
    {
        var queue = new CombatQueue();

        var attacker = context.attacker;
        var defender = context.defender;
        var atkWeapon = context.attackerWeapon;
        var defWeapon = context.defenderWeapon;

        // This is where all the combat actions get queued up for a single context, so its not so hard coded no more.
        queue.AddAction(new CombatAction(attacker, defender, atkWeapon));

        if(defWeapon != null && InRange(defender, attacker, defWeapon) && defender.currentHP > 0)
        {
            queue.AddAction(new CombatAction(defender, attacker, defWeapon, isCounter: true));
        }

        if(defender.currentHP > 0 && attacker.GetModifiedStat(StatType.SPD) - defender.GetModifiedStat(StatType.SPD) >= followUpSpeedThreshold)
        {
            queue.AddAction(new CombatAction(attacker, defender, atkWeapon, isFollowUp: true));
        }

        return queue;
    }

    public static void ResolveAttack(CombatAction action, CombatContext context)
    {
        var attacker = action.attacker;
        var defender = action.defender;
        var weapon = action.weapon;

        int attackPower = weapon.damageType == DamageType.Physical
            ? attacker.GetModifiedStat(StatType.STR)
            : attacker.GetModifiedStat(StatType.ARC);

        int defensePower = weapon.damageType == DamageType.Physical
            ? defender.GetModifiedStat(StatType.DEF)
            : defender.GetModifiedStat(StatType.RES);

        context.baseDamage = Mathf.Max(0, attackPower - defensePower);
        context.hitRate = attacker.hit;
        context.avoid = defender.avoidance;
        context.hitChance = Mathf.Clamp(context.hitRate - context.avoid, 0, 100);

        context.critRate = attacker.crit;
        context.critAvoid = Mathf.FloorToInt(defender.GetModifiedStat(StatType.LCK) / 3f);
        context.critChance = Mathf.Clamp(context.critRate - context.critAvoid, 0, 100);

        context.hitting = attacker.Roll(context.hitChance);
        context.critting = attacker.Roll(context.critChance);
        context.finalDamage = 0;

        if (context.hitting)
        {
            EventSystem.TriggerEvent(attacker, defender, Event.OnHit, context);
            if (context.critting)
            {
                EventSystem.TriggerEvent(attacker, defender, Event.OnCrit, context);
                context.finalDamage = Mathf.FloorToInt((context.baseDamage + context.bonusDamage) * context.critPower);
            }
            else
            {
                context.finalDamage = context.baseDamage + context.bonusDamage;
            }

            defender.TakeDamage(context.finalDamage);
            Debug.Log($"{attacker.unitName} deals {context.finalDamage} damage to {defender.unitName}" +
                      (context.critting ? " (Crit!)" : ""));
        }
        else
        {
            Debug.Log($"{attacker.unitName} misses {defender.unitName}");
        }
    }

    private static bool InRange(Unit attacker, Unit target, WeaponItem weapon)
    {
        int dist = Mathf.Abs(attacker.GridPosition.x - target.GridPosition.x) +
                    Mathf.Abs(attacker.GridPosition.y - target.GridPosition.y);
        
        return dist >= weapon.minRange && dist <= weapon.maxRange;
    }
}

public class CombatAction
{
    public Unit attacker;
    public Unit defender;
    public WeaponItem weapon;
    public bool isCounter;
    public bool isFollowUp;

    public CombatAction(Unit attacker, Unit defender, WeaponItem weapon, bool isCounter = false, bool isFollowUp = false)
    {
        this.attacker = attacker;
        this.defender = defender;
        this.weapon = weapon;
        this.isCounter = isCounter;
        this.isFollowUp = isFollowUp;
    }
}

public class CombatQueue
{
    public List<CombatAction> actions = new();

    public void AddAction(CombatAction action)
    {
        actions.Add(action);
    }

    public void ResolveAll(CombatContext context)
    {
        foreach (var action in actions)
        {
            CombatSystem.ResolveAttack(action, context);
        }

        EventSystem.TriggerEvent(context.attacker, context.defender, Event.OnCombatEnd, context);
    }
}

public static class CombatPreviewHelper
{
    public static void GetCombatPreview(Unit attacker, Unit defender, WeaponItem weapon,
        out int baseDamage, out int bonusDamage, out int hit, out int crit)
    {
        baseDamage = 0;
        bonusDamage = 0;
        hit = 0;
        crit = 0;

        if(weapon == null) return;

        int attackStat = weapon.damageType == DamageType.Physical
            ? attacker.GetModifiedStat(StatType.STR)
            : attacker.GetModifiedStat(StatType.ARC);

        int defenseStat = weapon.damageType == DamageType.Physical
            ? defender.GetModifiedStat(StatType.DEF)
            : defender.GetModifiedStat(StatType.RES);
        
        baseDamage = Mathf.Max(0, attackStat - defenseStat);
        bonusDamage = 0;

        hit = Mathf.Clamp(attacker.hit - defender.avoidance, 0, 100);
        crit = Mathf.Clamp(attacker.crit - Mathf.FloorToInt(defender.GetModifiedStat(StatType.LCK) / 3f), 0, 100);
    }

    public static string FormatCombatText(int baseDmg, int bonusDmg, int hit, int crit)
    {
        return $"Dmg: {baseDmg} + {bonusDmg}\nHit: {hit}%\nCrit: {crit}%";
    }
}
