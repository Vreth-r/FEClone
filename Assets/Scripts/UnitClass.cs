using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitClass", menuName = "Tactics RPG/Unit Class")]
public class UnitClass : ScriptableObject
{
    public string className; // name
    public string classDescription; // description
    public int movementRange; // movement allowed
    public List<WeaponType> allowedWeapons; // weapons allowed to be used
    public List<ClassTag> classTags; // whether its flying, armored, etc

    // Growth rates (0-100%) chance of skill leveling on level up
    [Range(0, 100)] public int hpGrowth; 
    [Range(0, 100)] public int strengthGrowth;
    [Range(0, 100)] public int arcaneGrowth;
    [Range(0, 100)] public int defenseGrowth;
    [Range(0, 100)] public int speedGrowth;
    [Range(0, 100)] public int skillGrowth;
    [Range(0, 100)] public int resistanceGrowth;
    [Range(0, 100)] public int luckGrowth;

    // Will def add more this is good 4 now

    public List<LevelSkill> skillsByLevel; // Skills learned at specific levels 

    [Header("Promotion")]
    public UnitClass promotedClass; // class that it can promote to (will switch to be multiple later)
    public bool canPromote = false;
    public int promotionLevel = 10; // default level to allow for a promotion
}

public enum WeaponType // placeholder for now, will be made its own class soon
{
    Sword, Axe, Lance, Bow, Magic, Staff
}

public enum ClassTag
{
    Armored, Mounted, Flying, Infantry, Magical, Wyrm, Beast
}

[System.Serializable]
public class LevelSkill // also a placeholder for now, skills are very in depth
{
    public int level;
    public Skill skill;
}
