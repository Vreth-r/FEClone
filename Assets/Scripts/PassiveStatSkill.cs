using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tactics RPG/Skill/Passive Stat Skill")]
public class PassiveStatSkill : Skill
{
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

    public override void Apply(Unit unit)
    {
        unit.statBonuses.AddStatBonus(this);
    }

    public void UnApply(Unit unit)
    {
        unit.statBonuses.RemoveStatBonus(this);
    }
}
