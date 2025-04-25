using UnityEngine;

[System.Serializable]
public class StatScalingModifier
{
    public StatType targetStat;
    public StatType sourceStat;
    public float multiplier;
}

public enum StatType
{
    STR,
    ARC,
    SPD,
    DEF,
    SKL,
    RES,
    LCK,
    CHP,
    MHP,
    AVO,
    CRI,
    HIT
}