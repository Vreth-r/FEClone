using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public abstract void Apply(Unit source, Unit target, EffectContext context);

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
}