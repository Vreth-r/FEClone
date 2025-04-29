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
    public int avoidance; // avoidance affects how likely the unit is to avoid an attack (attacker hit - def avoid = %chance to hit)
    public int crit; // base % chance to deal a critical hit (damage increase depends on class)
    public int hit; // hit affects how likely the unit is to actually hit the unit they are attacking (see avoidance)

    // Skills
    public List<Skill> skills = new();
    public StatBonusSet statBonuses;

    // Inventory 
    public List<Item> inventory = new(); // thinking of making a class for this but the inventory is so simple anyway
    public Item equippedItem; // the current equipped item (will always be a weapon)
    public WeaponProficiency proficiencyLevels;

    public Vector2Int GridPosition { get; set; }

    private void Start()
    {   
        statBonuses = new StatBonusSet();
        // Start will run at the start of EVERY start, even if booting into a save
        GridPosition = (Vector2Int)GridManager.Instance.WorldToCell(transform.position);
        movementRange = unitClass.movementRange;
        UnitManager.Instance.RegisterUnit(this); // Tell the unit manager this thing exists
        proficiencyLevels.Initialize();
        unitClass.proficiencies.Initialize();
        RefreshStats();
        CalculateStats();
        proficiencyLevels.AddProficienciesFromOther(unitClass.proficiencies);
        if (inventory.Count != 0) Equip(inventory[0]); // equip the first thing in the inventory(dev)
    }

    public void RefreshStats()
    {
        statBonuses.Clear();
        ApplyPassiveEffects();
    }
    public void ApplyPassiveEffects()
    {   
        // Will change this to have a trigger param and make it general
        var context = new EffectContext();

        // process skills
        foreach (var skill in skills)
        {
            foreach (var effectInstance in skill.effects)
            {
                effectInstance.TryApply(this, this, EffectTrigger.Passive, context);
            }
        }

        if (equippedItem is WeaponItem weapon)
        {
            foreach (var effectInstance in weapon.effects)
            {
                effectInstance.TryApply(this, this, EffectTrigger.Passive, context);
            }
        }
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
        CalculateStats();

        // Check for new skills
        foreach (var skillEntry in unitClass.skillsByLevel)
        {
            if (skillEntry.level == level && !skills.Contains(skillEntry.skill))
            {
                skills.Add(skillEntry.skill);
                Debug.Log($"{unitName} learned skill: {skillEntry.skill.skillName}");
            }
        }
    }

    private bool Roll(int percent)
    {
        return Random.Range(0, 100) < percent;
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
        RefreshStats();
        proficiencyLevels.AddProficienciesFromOther(unitClass.proficiencies);
        // add support for multiple promotion classes later
        Debug.Log($"{unitName} promoted to {unitClass.className}!");

        // add stat boosts on promo here later
    }

    public int GetStatByType(StatType stat)
    {
        return stat switch
        {
            StatType.MHP => maxHP,
            StatType.CHP => currentHP,
            StatType.STR => strength,
            StatType.ARC => arcane,
            StatType.DEF => defense,
            StatType.SPD => speed,
            StatType.SKL => skill,
            StatType.RES => resistance,
            StatType.LCK => luck,
            StatType.AVO => avoidance,
            StatType.CRI => crit,
            StatType.HIT => hit,
            _ => 0
        };
    }

    public int GetModifiedStat(StatType stat)
    {   
        int flatBonus = 0; // init the flat bonus
        float scalingBonus = 0; // init the mult bonus

        // base stats
        int baseStatValue = GetStatByType(stat);
    

        flatBonus = statBonuses.GetFlatMod(stat);
        scalingBonus = statBonuses.GetMultMod(stat) * baseStatValue;

        //  apply the weapon bonus 
        if (equippedItem is WeaponItem weapon)
        {
            flatBonus += stat switch
            {
                //"MHP" => weapon.bonusHP, // this one should never proc but just in case
                StatType.STR => weapon.bonusStrength,
                StatType.ARC => weapon.bonusArcane,
                StatType.DEF => weapon.bonusDefense,
                StatType.SPD => weapon.bonusSpeed,
                StatType.SKL => weapon.bonusSkill,
                StatType.RES => weapon.bonusResistance,
                StatType.LCK => weapon.bonusLuck,
                _ => 0
            };
        }
        
        foreach (var mod in statBonuses.crossStatModifiers) // go over all the modifiers
        {
            if (mod.targetStat != stat) continue; // skip the ones we dont care about
            float sourceValue = GetStatByType(mod.sourceStat); // get the base value of the source stat
            scalingBonus += sourceValue * mod.multiplier; // add to the scaling bonus of the in question stat the extra value
        }
        return Mathf.FloorToInt(baseStatValue + flatBonus + scalingBonus);
        // i am intentionally not just straight changing the units stats because i want to show the 
        // total amount of bonus stat they have compared to their base later
    }

    public void CalculateStats()
    {
        int tempAvoid = Mathf.CeilToInt((float)(GetModifiedStat(StatType.SPD)*1.5) + (GetModifiedStat(StatType.LCK)/2)); // get the base values from primary stats
        int tempHit = Mathf.CeilToInt((float)(GetModifiedStat(StatType.SKL)*1.5) + (GetModifiedStat(StatType.LCK)/2));
        int tempCrit = Mathf.CeilToInt((float)(GetModifiedStat(StatType.SKL)/2));

        int weaponAvoidBonus = 0; // instantiate these for scope
        int weaponHitBonus = 0;
        int weaponCritBonus = 0;

        if(equippedItem is WeaponItem weapon)
        {
            weaponAvoidBonus = weapon.avoid; // if weapon is valid, set the bonus stats
            weaponHitBonus = weapon.hit;
            weaponCritBonus = weapon.crit;
        }


        avoidance = tempAvoid + weaponAvoidBonus + statBonuses.GetFlatMod(StatType.AVO); // combine them all including skill bonuses and set the stats
        hit = tempHit + weaponHitBonus + statBonuses.GetFlatMod(StatType.HIT);
        crit = tempCrit + weaponCritBonus + statBonuses.GetFlatMod(StatType.CRI);
    }
    public void Equip(Item item)
    {
        RefreshStats();
        if (item.itemType != ItemType.Weapon)
        {
            Debug.Log("Cannot Equip non-weapon item.");
            // add functionality later
            return;
        }

        var weapon = item as WeaponItem; // HOLY coding
        weapon.proficiency.Initialize();
        if (!proficiencyLevels.CheckWeapon(weapon) || !unitClass.proficiencies.HasProficiency(weapon.weaponType))
        {
            Debug.Log("Cannot use this type of weapon");
            // functionality later
            return;
        }

        equippedItem = item;
        attackRange = item.maxRange;
        CalculateStats();
        Debug.Log($"{unitName} equipped {item.itemName}");
    }

    public void UnEquip(Item item)
    {
        RefreshStats();
        if(equippedItem == null || equippedItem != item) return; // cant unequip nothing or what you dont have equipped!
        equippedItem = null;
        CalculateStats();
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
