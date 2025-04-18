using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team { Player, Enemy }
public class Unit : MonoBehaviour
{
    public Team team; // teamwork makes the dream work
    public UnitClass unitClass; // class the unit belongs to
    public string unitName; // name of the unit in game, like Corrin, Robin, etc
    public string unitDescription; // small description of their personality because fire emblem is actually a dating sim
    public string unitTitle; // FE Heros had this title system like "Robin: Fell Vessel" and i think thats cool so fuck you

    // Main Stats (Mostly set in editor to start with)
    public int movementRange; // how far the unit can mave based off its class
    public int attackRange = 0; // default to 0, changes with equipped weapon
    public int level = 1; // class level (deciding on max because still deciding on how i want classes to promote)
    public int currentHP; // working hp
    public int maxHP; // max hp memory
    public int strength; // affects how hard unit hits for physical attacks
    public int arcane; // affects how hard unit hits for magic attacks
    public int defense; // affects how much physical damage a unit can shrug off
    public int speed; // affects how fast the unit (hitting chance and avoidance) is and if they can hit twice (5 more speed than op)
    public int skill; // affects crit chance and a lot of skill triggers
    public int resistance; // affects how much magical damage a unit can shrug off
    public int luck; // affects many things, mainly skill triggers and avoidances
    // will be adding MANY more stats theres a lot behind the scenes but these stats are what everyone can see up front

    // Secondary Stats
    public double avoidance; // avoidance affects how likely the unit is to avoid an attack (attacker hit - def avoid = %chance to hit)
    public double crit; // base % chance to deal a critical hit (damage increase depends on class)
    public double hit; // hit affects how likely the unit is to actually hit the unit they are attacking (see avoidance)

    // Skills
    public List<Skill> learnedSkills = new();

    // Inventory 
    public List<Item> inventory = new(); // thinking of making a class for this but the inventory is so simple anyway
    public Item equippedItem;

    public Vector2Int GridPosition { get; set; }

    private void Start()
    {
        GridPosition = (Vector2Int)GridManager.Instance.WorldToCell(transform.position);
        movementRange = unitClass.movementRange;
        UnitManager.Instance.RegisterUnit(this); // Tell the unit manager this thing exists
        Equip(inventory[0]);
        CalculateSecondaryStats();
    }

    public void LevelUp()
    {
        level++; // congrats!

        // Growth Rolls
        if (Roll(unitClass.hpGrowth)) maxHP++;
        if (Roll(unitClass.strengthGrowth)) strength++;
        if (Roll(unitClass.arcaneGrowth)) arcane++;
        if (Roll(unitClass.defenseGrowth)) defense++;
        if (Roll(unitClass.speedGrowth)) speed++;
        if (Roll(unitClass.skillGrowth)) skill++;
        if (Roll(unitClass.resistanceGrowth)) resistance++;
        if (Roll(unitClass.luckGrowth)) luck++;
        CalculateSecondaryStats();

        // Check for new skills
        foreach (var skillEntry in unitClass.skillsByLevel)
        {
            if (skillEntry.level == level && !learnedSkills.Contains(skillEntry.skill))
            {
                learnedSkills.Add(skillEntry.skill);
                Debug.Log($"{unitName} learned skill: {skillEntry.skill.skillName}");
            }
        }
    }

    private bool Roll(int percent)
    {
        return Random.Range(0, 100) < percent;
    }

    public bool CanUseWeapon(WeaponType weapon)
    {
        return unitClass.allowedWeapons.Contains(weapon);
    }

    public bool HasTag(ClassTag tag)
    {
        return unitClass.classTags.Contains(tag);
    }

    public int GetMovementRange()
    {
        return unitClass.movementRange;
    }

    public bool canPromote()
    {
        // a unit can only promote when its at the level and when theres even a class to promote to
        return unitClass.canPromote && level >= unitClass.promotionLevel && unitClass.promotedClass != null;
    }

    public void Promote()
    {
        if (!canPromote()) return; // sorry bud get better

        unitClass = unitClass.promotedClass; // promote to the promotion class
        // add support for multiple promotion classes later
        Debug.Log($"{unitName} promoted to {unitClass.className}!");

        // add stat boosts on promo here later
    }

    public int GetModifiedStat(int baseValue, string statName)
    {
        if (equippedItem is not WeaponItem weapon) return baseValue;

        return statName switch
        {
            "STR" => baseValue + weapon.bonusStrength,
            "ARC" => baseValue + weapon.bonusArcane,
            "DEF" => baseValue + weapon.bonusDefense,
            "SPD" => baseValue + weapon.bonusSpeed,
            "SKL" => baseValue + weapon.bonusSkill,
            "RES" => baseValue + weapon.bonusResistance,
            "LCK" => baseValue + weapon.bonusLuck,
            _ => baseValue
        };
    }

    public void CalculateSecondaryStats()
    {
        if(equippedItem is WeaponItem weapon)
        {
            avoidance = (GetModifiedStat(speed, "SPD")*1.5) + (GetModifiedStat(luck, "LCK")/2) + weapon.avoid;
            hit = (GetModifiedStat(skill, "SKL")*1.5) + (GetModifiedStat(luck, "LCK")/2) + weapon.hit;
            crit = (GetModifiedStat(skill, "SKL")/2) + weapon.crit;
        }
        else 
        {
            avoidance = (GetModifiedStat(speed, "SPD")*1.5) + (GetModifiedStat(luck, "LCK")/2);
            hit = (GetModifiedStat(skill, "SKL")*1.5) + (GetModifiedStat(luck, "LCK")/2);
            crit = (GetModifiedStat(skill, "SKL")/2);
        }
    }
    public void Equip(Item item)
    {
        if (item.itemType != ItemType.Weapon)
        {
            Debug.Log("Cannot Equip non-weapon item.");
            // add functionality later
            return;
        }

        var weapon = item as WeaponItem; // HOLY coding
        if (!CanUseWeapon(weapon.weaponType))
        {
            Debug.Log("Cannot use this type of weapon");
            // functionality later
            return;
        }

        equippedItem = item;
        attackRange = item.maxRange;
        CalculateSecondaryStats();
        Debug.Log($"{unitName} equipped {item.itemName}");
    }

    public void UnEquip(Item item)
    {
        if(equippedItem == null || equippedItem != item) return; // cant unequip nothing or what you dont have equipped!
        equippedItem = null;
        CalculateSecondaryStats();
    }

    public void UseItem(int index, Unit target = null)
    {   
        // validate the index
        if(index < 0 || index >= inventory.Count) return; 

        var item = inventory[index];
        item.Use(this, target ?? this); // default use on self
    }

    public void AddItem(Item item)
    {
        if(inventory.Count >= 5)
        {
            Debug.Log("Inventory full bruh.");
            return;
        }

        inventory.Add(item);
    }

    public void RemoveItem(Item item)
    {
        if (inventory.Contains(item)) inventory.Remove(item);
    }
}
