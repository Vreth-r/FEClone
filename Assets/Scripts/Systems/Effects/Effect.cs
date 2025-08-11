using UnityEngine;

/// <summary>
/// Abtract Effect class acts as a container for backend per-effect code.
/// Intended to be referenced in tandem with more information in an EffectInstance
/// </summary>
public abstract class Effect : ScriptableObject
{
    public abstract void Apply(Unit source, Unit target, EffectContext context); // abstract application method

    // Lookup tables for child classes
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

    // lookup table for StatModifiers.cs
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