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
    public int movementRange; // how far the unit can mave based off its class
    public int attackRange = 2; // will prob be changed later when weapons are implemented as this will be variable depending on weapon type
    public int level = 1; // class level (deciding on max because still deciding on how i want classes to promote)
    public int currentHP; // working hp
    public int maxHP; // max hp memory
    public int strength; // affects how hard unit hits for physical attacks
    public int arcane; // affects how hard unit hits for magic attacks
    public int defense; // affects how much physical damage a unit can shrug off
    public int speed; // affects how fast the unit (hitting change and avoidance) is and if they can hit twice (5 more speed than op)
    public int skill; // affects crit chance and a lot of skill triggers
    public int resistance; // affects how much magical damage a unit can shrug off
    public int luck; // affects many things, mainly skill triggers and avoidances
    // will be adding MANY more stats theres a lot behind the scenes but these stats are what everyone can see up front

    public List<Skill> learnedSkills = new();

    public Vector2Int GridPosition { get; set; }

    private void Start()
    {
        GridPosition = (Vector2Int)GridManager.Instance.WorldToCell(transform.position);
        movementRange = unitClass.movementRange;
        UnitManager.Instance.RegisterUnit(this); // Tell the unit manager this thing exists
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

        // Check for new skills
        foreach (var skillEntry in unitClass.skillsByLevel)
        {
            if (skillEntry.level == level && !learnedSkills.Contains(skillEntry.skill))
            {
                learnedSkills.Add(skillEntry.skill);
                Debug.Log($"{name} learned skill: {skillEntry.skill.skillName}");
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
        Debug.Log($"{name} promoted to {unitClass.className}!");

        // add stat boosts on promo here later
    }
}
