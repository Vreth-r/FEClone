public class StatBonusSet
{
    // Class to keep track of stat bonuses for units
    // used for passive skills and weapons i think
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

    public void AddStatBonus(PassiveStatSkill skill)
    {
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
    }

    public void RemoveStatBonus(PassiveStatSkill skill)
    {
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
    }

    public void Clear()
    {
        bonusStrength = bonusArcane = bonusDefense = bonusSpeed
         = bonusSkill = bonusResistance = bonusLuck = bonusAvoid
         = bonusCrit = bonusHit = 0;
    }
}
