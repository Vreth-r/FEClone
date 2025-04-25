using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tactics RPG/Effects/Passive Stat Bonus")]
public class PassiveStatEffect : Effect
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

    public override void ApplyPassive(Unit unit)
    {
        unit.statBonuses.AddStatBonus(this);    
    }

    public void UnApply(Unit unit)
    {
        unit.statBonuses.RemoveStatBonus(this);
    }
}