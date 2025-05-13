using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// still need to add stuff like
/*
    - terrain bonuses
    - animations (later way later)
*/

/* CombatSystem handles all the actual calculations and rolls required for combat and sets all the data behind the scenes
    When combat is called, it initiates a combat context (which is just a variable holder really) and decides the combat sequence events
    When told by the combat scene manager, it does all combat calculations and sets units stats accordingly.
*/

// Resolve attack is called in the combat scene manager
public class CombatSystem
{
    // Constants
    private const int followUpSpeedThreshold = 5;
    private const float WeaponAdvantageDamageMult = 1.2f;
    private const float WeaponDisadvantageDamageMult = 0.8f;
    private const int WeaponAdvantageHitBonus = 10;
    private const int WeaponDisadvantageHitPenalty = -10;

    // This gets called whenever combat is to be initiated
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
        context.attackerPrevHP = context.attacker.currentHP; // cache old HP values for visuals
        context.defenderPrevHP = context.defender.currentHP;

        // if the attacker speed is 5 greater than the defender, the attacker gets to make a followup attack
        context.isFollowingUp = context.attacker.GetModifiedStat(StatType.SPD) - context.defender.GetModifiedStat(StatType.SPD) >= followUpSpeedThreshold;

        EventSystem.TriggerEvent(context.attacker, context.defender, Event.OnCombatStart, context); // trigger combat start event

        var queue = BuildCombatQueue(context); // build the combat event queue
        CombatSceneManager.Instance.EnterCombatScene(attacker, defender, context, queue); // tell the scene manager to start the scene
    }

    // builds a combat event queue based off many factors
    public static CombatQueue BuildCombatQueue(CombatContext context)
    {
        var queue = new CombatQueue(); // init queue

        var attacker = context.attacker; // set shorthands
        var defender = context.defender;
        var atkWeapon = context.attackerWeapon;
        var defWeapon = context.defenderWeapon;

        // Add the initial combat action, guaranteed here because you cant get to this code without targetting and range checks prior
        queue.AddAction(new CombatAction(attacker, defender, atkWeapon, defWeapon));

        if(defWeapon != null && InRange(defender, attacker, defWeapon) && defender.currentHP > 0) // if the defender has a weapon and is in range and is alive, they get to counter attack
        {
            queue.AddAction(new CombatAction(defender, attacker, defWeapon, atkWeapon, isCounter: true));
        }

        if(defender.currentHP > 0 && context.isFollowingUp) // if the defender is alive and the unit is able to followup, they get to followup attack.
        {
            queue.AddAction(new CombatAction(attacker, defender, atkWeapon, defWeapon, isFollowUp: true));
        }

        return queue;
    }

    // Resolves a combat action given a combat context
    public static void ResolveAttack(CombatAction action, CombatContext context)
    {
        var attacker = action.attacker; // set shorthands
        var defender = action.defender;
        var weapon = action.attackerWeapon;
        var defWeapon = action.defenderWeapon;

        context.attackerHasWeaponAdvantage = false; // set weapon flags for scene manager
        context.attackerHasWeaponDisadvantage = false;
        context.attackerHasClassAdvantage = false;
        context.attackerHasClassDisadvantage = false;
        context.hitRateBonus = 0; // reset this in case of previous action changes.

        int attackPower = weapon.damageType == DamageType.Physical // determine which offensive stat is being used
            ? attacker.GetModifiedStat(StatType.STR)
            : attacker.GetModifiedStat(StatType.ARC);

        int defensePower = weapon.damageType == DamageType.Physical // determine which defensive stat is being used
            ? defender.GetModifiedStat(StatType.DEF)
            : defender.GetModifiedStat(StatType.RES);

        context.baseDamage = Mathf.Max(0, attackPower - defensePower); // calculate the base damage

        if(defWeapon != null) // if the defender has a weapon
        {
            // Dont know if I love the advantage/disadvantage being multiplicative, might change
            if(weapon.IsEffectiveAgainstWeapon(defWeapon.weaponType)) // check if attacker has weapon advantage over defender
            {
                Debug.Log($"{attacker.unitName}'s weapon is strong against {defender.unitName}'s weapon");
                context.damageMult *= WeaponAdvantageDamageMult; // increase the damage multiplier
                context.hitRateBonus += WeaponAdvantageHitBonus; // add the hit bonus
                context.attackerHasWeaponAdvantage = true; // set flag for the scene manager
            }
            else if(weapon.IsWeakToWeapon(defWeapon.weaponType)) // check if attacker has weapon disadvantage over defender
            {
                Debug.Log($"{attacker.unitName}'s weapon is weak against {defender.unitName}'s weapon");
                context.damageMult *= WeaponDisadvantageDamageMult; // see above 
                context.hitRateBonus += WeaponDisadvantageHitPenalty;
                context.attackerHasWeaponDisadvantage = true;
            }

            foreach(var tag in defender.unitClass.classTags) // for every class tag the defender has
            {
                if(weapon.IsEffectiveAgainstClass(tag)) // check if attacker has weapon advantage over defender class tags
                {
                    Debug.Log($"{attacker.unitName}'s weapon is strong against {defender.unitName}'s class");
                    context.damageMult *= WeaponAdvantageDamageMult; // see above
                    context.attackerHasClassAdvantage = true;
                }
                else if(weapon.IsWeakToClass(tag)) // check if attacker has weapon diadvantage over defender class tags
                {
                    Debug.Log($"{attacker.unitName}'s weapon is weak against {defender.unitName}'s class");
                    context.damageMult *= WeaponDisadvantageDamageMult; // see above
                    context.attackerHasClassDisadvantage = true;
                }
            }
        }

        context.hitRate = attacker.hit + context.hitRateBonus; // calculate the hit chance 
        context.avoid = defender.avoidance;
        context.hitChance = Mathf.Clamp(context.hitRate - context.avoid, 0, 100);

        context.critRate = attacker.crit; // calculate the crit chance
        context.critAvoid = Mathf.FloorToInt(defender.GetModifiedStat(StatType.LCK) / 3f);
        context.critChance = Mathf.Clamp(context.critRate - context.critAvoid, 0, 100);

        context.hitting = attacker.Roll(context.hitChance); // roll both the hit and crit
        context.critting = attacker.Roll(context.critChance);
        context.finalDamage = 0; // reset the final damage if there was an action before

        if (context.hitting)
        {
            EventSystem.TriggerEvent(attacker, defender, Event.OnHit, context); // trigger on hit event
            if (context.critting)
            {
                EventSystem.TriggerEvent(attacker, defender, Event.OnCrit, context); // trigger on crit event (will get both events)
                context.finalDamage = Mathf.FloorToInt(((context.baseDamage + context.bonusDamage) * context.damageMult) * context.critPower); // calculate final damage (critting)
            }
            else
            {
                context.finalDamage = Mathf.FloorToInt((context.baseDamage + context.bonusDamage) * context.damageMult); // calculate final damage (not critting)
            }

            defender.TakeDamage(context.finalDamage); // defender actually takes the damage
            Debug.Log($"{attacker.unitName} deals {context.finalDamage} damage to {defender.unitName}" +
                      (context.critting ? " (Crit!)" : ""));
        }
        else
        {
            Debug.Log($"{attacker.unitName} misses {defender.unitName}"); // whiff
        }
    }

    private static bool InRange(Unit attacker, Unit target, WeaponItem weapon)
    {
        int dist = Mathf.Abs(attacker.GridPosition.x - target.GridPosition.x) +
                    Mathf.Abs(attacker.GridPosition.y - target.GridPosition.y);
        
        return dist >= weapon.minRange && dist <= weapon.maxRange;
    }
}

// a combat action is a single(without skills or effects) strike between an attacker and defender
public class CombatAction
{
    public Unit attacker;
    public Unit defender;
    public WeaponItem attackerWeapon;
    public WeaponItem defenderWeapon;
    public bool isCounter;
    public bool isFollowUp;

    public CombatAction(Unit attacker, Unit defender, WeaponItem attackerWeapon, WeaponItem defenderWeapon, bool isCounter = false, bool isFollowUp = false)
    {
        this.attacker = attacker;
        this.defender = defender;
        this.attackerWeapon = attackerWeapon;
        this.defenderWeapon = defenderWeapon;
        this.isCounter = isCounter;
        this.isFollowUp = isFollowUp;
    }
}

// A queue for combat actions to be resolved in order.
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

// A preview helper will forecast the basic, guaranteed damage a unit will deal to another should they strike them.
public static class CombatPreviewHelper
{
    public static void GetCombatPreview(Unit attacker, Unit defender, WeaponItem weapon,
        out int baseDamage, out int bonusDamage, out int hit, out int crit)
    {
        baseDamage = 0; // attack power - defense power
        bonusDamage = 0; // weapon advantages/disadvantages, etc
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
