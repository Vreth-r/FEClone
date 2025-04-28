using UnityEngine;

public class StatScalingModifier
{
    public StatType targetStat;
    public StatType sourceStat;
    public float multiplier;

    public StatScalingModifier(StatType tStat, StatType sStat, float mult)
    {
        targetStat = tStat;
        sourceStat = sStat;
        multiplier = mult;
    }
}