using UnityEngine;

[System.Serializable]
public class StatModifier
{   
    public StatModType modType;
    public StatType targetStat;
    public StatType sourceStat;
    public float multiplier;
    public int flatValue;

    public int turnCount; //for effects that expire after a certain number of turns.

    public ExpireType expireCond;

    public StatModifier(StatModType modType, StatType targetStat, StatType sourceStat, float multiplier, int flatValue, ExpireType expire = ExpireType.UnEquip, int turnCount = -1)
    {
        this.modType = modType;
        this.targetStat = targetStat;
        this.sourceStat = sourceStat;
        this.multiplier = multiplier;
        this.flatValue = flatValue;
        this.expireCond = expire;
        this.turnCount = turnCount;
    }

    public StatModifier(StatModType modType, StatType targetStat, int flatValue, ExpireType expire = ExpireType.UnEquip, int turnCount = -1)
    {
        this.modType = modType;
        this.targetStat = targetStat;
        this.flatValue = flatValue;
        this.expireCond = expire;
        this.turnCount = turnCount;
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