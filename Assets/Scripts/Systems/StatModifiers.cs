using UnityEngine;

[System.Serializable]
public class StatModifier
{   
    public StatModType modType;
    public StatType targetStat;
    public StatType sourceStat;
    public float multiplier;
    public int flatValue;

    public ExpireType expireCond;

    public StatModifier(StatModType modType, StatType targetStat, StatType sourceStat, float multiplier, int flatValue, ExpireType expire = ExpireType.UnEquip)
    {
        this.modType = modType;
        this.targetStat = targetStat;
        this.sourceStat = sourceStat;
        this.multiplier = multiplier;
        this.flatValue = flatValue;
        this.expireCond = expire;
    }

    public StatModifier(StatModType modType, StatType targetStat, int flatValue, ExpireType expire = ExpireType.UnEquip)
    {
        this.modType = modType;
        this.targetStat = targetStat;
        this.flatValue = flatValue;
        this.expireCond = expire;
    }
}

public enum StatModType
{
    Flat,
    Multiplier,
    CrossStat
}

public enum ExpireType
{
    Combat,
    Turn,
    UnEquip,
    Passive
}