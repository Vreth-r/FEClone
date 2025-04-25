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

    public void AddStatBonus(PassiveStatEffect effect)
    {
        bonusHP += effect.bonusHP;
        bonusStrength += effect.bonusStrength;
        bonusArcane += effect.bonusArcane;
        bonusDefense += effect.bonusDefense;
        bonusSpeed += effect.bonusSpeed;
        bonusSkill += effect.bonusSkill;
        bonusResistance += effect.bonusResistance;
        bonusLuck += effect.bonusLuck;
        bonusAvoid += effect.bonusAvoid;
        bonusCrit += effect.bonusCrit;
        bonusHit += effect.bonusHit;

        bonusHPMod += effect.bonusHPMod;
        bonusStrengthMod += effect.bonusStrengthMod;
        bonusArcaneMod += effect.bonusArcaneMod;
        bonusDefenseMod += effect.bonusDefenseMod;
        bonusSpeedMod += effect.bonusSpeedMod;
        bonusSkillMod += effect.bonusSkillMod;
        bonusResistanceMod += effect.bonusResistanceMod;
        bonusLuckMod += effect.bonusLuckMod;
        bonusAvoidMod += effect.bonusAvoidMod;
        bonusCritMod += effect.bonusCritMod;
        bonusHitMod += effect.bonusHitMod;

        crossStatModifiers.AddRange(effect.statScalingModifiers);
    }

    public void RemoveStatBonus(PassiveStatEffect effect)
    {
        bonusHP -= effect.bonusHP;
        bonusStrength -= effect.bonusStrength;
        bonusArcane -= effect.bonusArcane;
        bonusDefense -= effect.bonusDefense;
        bonusSpeed -= effect.bonusSpeed;
        bonusSkill -= effect.bonusSkill;
        bonusResistance -= effect.bonusResistance;
        bonusLuck -= effect.bonusLuck;
        bonusAvoid -= effect.bonusAvoid;
        bonusCrit -= effect.bonusCrit;
        bonusHit -= effect.bonusHit;

        bonusHPMod -= effect.bonusHPMod;
        bonusStrengthMod -= effect.bonusStrengthMod;
        bonusArcaneMod -= effect.bonusArcaneMod;
        bonusDefenseMod -= effect.bonusDefenseMod;
        bonusSpeedMod -= effect.bonusSpeedMod;
        bonusSkillMod -= effect.bonusSkillMod;
        bonusResistanceMod -= effect.bonusResistanceMod;
        bonusLuckMod -= effect.bonusLuckMod;
        bonusAvoidMod -= effect.bonusAvoidMod;
        bonusCritMod -= effect.bonusCritMod;
        bonusHitMod -= effect.bonusHitMod;

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
