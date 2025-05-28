using UnityEngine;

// Abstract class for an Effect, more beef in EffectInstance
// gets subclassed and those subclasses get made into scriptable objects (might change that its p jank)

public abstract class Effect : ScriptableObject
{
    public abstract void Apply(Unit source, Unit target, EffectContext context); // abstract application method

    public StatType GetStatTypeFromName(string statName)
    {
        return statName switch
        {
            "MHP" => StatType.MHP,
            "CHP" => StatType.CHP,
            "STR" => StatType.STR,
            "ARC" => StatType.ARC,
            "DEF" => StatType.DEF,
            "SKL" => StatType.SKL,
            "SPD" => StatType.SPD,
            "RES" => StatType.RES,
            "LCK" => StatType.LCK,
            "AVO" => StatType.AVO,
            "HIT" => StatType.HIT,
            "CRI" => StatType.CRI,
            _ => StatType.NONE
        };
    }

    // See StatModifiers.cs
    public ExpireType GetExpireTypeFromString(string expireType)
    {
        return expireType switch
        {
            "COMBAT" => ExpireType.Combat,
            "TURN" => ExpireType.Turn,
            "UNEQUIP" => ExpireType.UnEquip,
            "PASSIVE" => ExpireType.Passive,
            _ => ExpireType.UnEquip
        };
    }
}