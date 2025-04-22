using System.Collections;
using System.Collections.Generic;
public class StatBonusSet
{
    // Class to keep track of stat bonuses for units
    // used for passive skills and weapons i think
    // Flat changes
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

    // multiplicative changes
    public float bonusHPMod;
    public float bonusStrengthMod;
    public float bonusArcaneMod;
    public float bonusDefenseMod;    
    public float bonusSpeedMod;
    public float bonusSkillMod;
    public float bonusResistanceMod;
    public float bonusLuckMod;
    public float bonusAvoidMod;
    public float bonusCritMod;
    public float bonusHitMod;

    // for use for skills modifying stats based off other or the same stats
    public List<StatScalingModifier> crossStatModifiers = new();

    public void AddStatBonus(PassiveStatSkill skill)
    {
        bonusHP += skill.bonusHP;
        bonusStrength += skill.bonusStrength;
        bonusArcane += skill.bonusArcane;
        bonusDefense += skill.bonusDefense;
        bonusSpeed += skill.bonusSpeed;
        bonusSkill += skill.bonusSkill;
        bonusResistance += skill.bonusResistance;
        bonusLuck += skill.bonusLuck;
        bonusAvoid += skill.bonusAvoid;
        bonusCrit += skill.bonusCrit;
        bonusHit += skill.bonusHit;

        bonusHPMod += skill.bonusHPMod;
        bonusStrengthMod += skill.bonusStrengthMod;
        bonusArcaneMod += skill.bonusArcaneMod;
        bonusDefenseMod += skill.bonusDefenseMod;
        bonusSpeedMod += skill.bonusSpeedMod;
        bonusSkillMod += skill.bonusSkillMod;
        bonusResistanceMod += skill.bonusResistanceMod;
        bonusLuckMod += skill.bonusLuckMod;
        bonusAvoidMod += skill.bonusAvoidMod;
        bonusCritMod += skill.bonusCritMod;
        bonusHitMod += skill.bonusHitMod;

        crossStatModifiers.AddRange(skill.statScalingModifiers);
    }

    public void RemoveStatBonus(PassiveStatSkill skill)
    {
        bonusHP -= skill.bonusHP;
        bonusStrength -= skill.bonusStrength;
        bonusArcane -= skill.bonusArcane;
        bonusDefense -= skill.bonusDefense;
        bonusSpeed -= skill.bonusSpeed;
        bonusSkill -= skill.bonusSkill;
        bonusResistance -= skill.bonusResistance;
        bonusLuck -= skill.bonusLuck;
        bonusAvoid -= skill.bonusAvoid;
        bonusCrit -= skill.bonusCrit;
        bonusHit -= skill.bonusHit;

        bonusHPMod -= skill.bonusHPMod;
        bonusStrengthMod -= skill.bonusStrengthMod;
        bonusArcaneMod -= skill.bonusArcaneMod;
        bonusDefenseMod -= skill.bonusDefenseMod;
        bonusSpeedMod -= skill.bonusSpeedMod;
        bonusSkillMod -= skill.bonusSkillMod;
        bonusResistanceMod -= skill.bonusResistanceMod;
        bonusLuckMod -= skill.bonusLuckMod;
        bonusAvoidMod -= skill.bonusAvoidMod;
        bonusCritMod -= skill.bonusCritMod;
        bonusHitMod -= skill.bonusHitMod;

        // need to add support to remove the cross stat multipliers
    }

    public void Clear()
    {
        bonusHP = bonusStrength = bonusArcane = bonusDefense = bonusSpeed
         = bonusSkill = bonusResistance = bonusLuck = bonusAvoid
         = bonusCrit = bonusHit = 0;

        bonusHPMod = bonusStrengthMod = bonusArcaneMod = bonusDefenseMod
        = bonusSpeedMod = bonusSkillMod = bonusResistanceMod = bonusLuckMod
        = bonusAvoidMod = bonusCritMod = bonusHitMod = 0;
        crossStatModifiers.Clear();
    }
}
