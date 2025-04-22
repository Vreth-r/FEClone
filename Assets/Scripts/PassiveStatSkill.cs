using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// it is here i gripe that i could have made an "effect" system and just assigned effects to skills and kept the system logic to use
// with items too. Im too far in to change it all now. If its neccessary i will but i dont see that happening.

[CreateAssetMenu(menuName = "Tactics RPG/Skill/Passive Stat Skill")]
public class PassiveStatSkill : Skill
{
    [Header("Flat Bonuses")]
    public int bonusHP;
    public int bonusStrength;
    public int bonusArcane;
    public int bonusDefense;
    public int bonusSpeed;
    public int bonusSkill;
    public int bonusResistance;
    public int bonusLuck;
    public int bonusAvoid;
    public int bonusCrit;
    public int bonusHit;

    [Header("Based off of other stat")]
    // like "gain ARC = MAX HP * 0.5"
    // modify targetStat = targetStat + [for(every stat: stat) stat*bonus[stat]Mod]
    [Range(0, 1)] public float bonusHPMod = 0f;
    [Range(0, 1)] public float bonusStrengthMod = 0f;
    [Range(0, 1)] public float bonusArcaneMod = 0f;
    [Range(0, 1)] public float bonusDefenseMod = 0f;
    [Range(0, 1)] public float bonusSpeedMod = 0f;
    [Range(0, 1)] public float bonusSkillMod = 0f;
    [Range(0, 1)] public float bonusResistanceMod = 0f;
    [Range(0, 1)] public float bonusLuckMod = 0f;
    [Range(0, 1)] public float bonusAvoidMod = 0f;
    [Range(0, 1)] public float bonusCritMod = 0f;
    [Range(0, 1)] public float bonusHitMod = 0f;
    
    [Header("Cross-Stat Scaling")]
    public List<StatScalingModifier> statScalingModifiers = new();

    public override void Apply(Unit unit)
    {
        unit.statBonuses.AddStatBonus(this);    
    }

    public void UnApply(Unit unit)
    {
        unit.statBonuses.RemoveStatBonus(this);
    }
}
