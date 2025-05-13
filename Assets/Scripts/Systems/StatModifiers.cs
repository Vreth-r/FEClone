using UnityEngine;

public abstract class StatModifier
{
    public StatType targetStat;
    public StatType sourceStat;
    public float multiplier;
    public int modifier;
}

// Intended to be passive/permanent modification to stats based off a percentage of another
public class CrossStatMultMod : StatModifier
{
    public CrossStatMultMod(StatType tStat, StatType sStat, float mult)
    {
        targetStat = tStat;
        sourceStat = sStat;
        multiplier = mult;
    }
}

// Intended to be a passive/permanent flat modification to a stat
public class FlatStatMod : StatModifier
{
    public FlatStatMod(StatType tStat, int mod)
    {
        targetStat = tStat;
        modifier = mod;
    }
}

// Intended to be a temporary in-combat instance-only flat modification of a stat
public class CombatStatMod : StatModifier
{
    public CombatStatMod(StatType tStat, int mod)
    {
        targetStat = tStat;
        modifier = mod;
    }
}