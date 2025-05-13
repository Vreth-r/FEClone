using UnityEngine;

[System.Serializable]
public class StatModifier
{   
    public StatModType modType;
    public StatType targetStat;
    public StatType sourceStat;
    public float multiplier;
    public int flatValue;

    public bool isTemporary; // gets cleared after combat

    public StatModifier(StatModType modType, StatType targetStat, StatType sourceStat, float multiplier, int flatValue, bool isTemporary = false)
    {
        this.modType = modType;
        this.targetStat = targetStat;
        this.sourceStat = sourceStat;
        this.multiplier = multiplier;
        this.flatValue = flatValue;
        this.isTemporary = isTemporary;
    }

    public StatModifier(StatModType modType, StatType targetStat, int flatValue, bool isTemporary = false)
    {
        this.modType = modType;
        this.targetStat = targetStat;
        this.flatValue = flatValue;
        this.isTemporary = isTemporary;
    }
}

public enum StatModType
{
    Flat,
    Multiplier,
    CrossStat
}