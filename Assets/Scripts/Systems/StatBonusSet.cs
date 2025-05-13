using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StatBonusSet
{
    private List<StatModifier> modifiers = new();

    public void AddModifier(StatModifier mod)
    {
        modifiers.Add(mod);
    }

    public void RemoveModifier(StatModifier mod)
    {
        modifiers.Remove(mod);
    }

    public void Clear()
    {
        modifiers.Clear();
    }

    public void ClearTemporaryModifiers()
    {
        modifiers.RemoveAll(m => m.isTemporary);
    }

    // computes total bonus for a given stat using all the mods
    public int GetTotalModifier(Unit unit, StatType targetStat)
    {
        float flat = 0;
        float mult = 0;
        float cross = 0;

        foreach(var mod in modifiers)
        {
            if(mod.targetStat != targetStat) continue;

            switch (mod.modType)
            {
                case StatModType.Flat:
                    flat += mod.flatValue;
                    break;
                
                case StatModType.Multiplier:
                    mult += mod.multiplier;
                    break;
                
                case StatModType.CrossStat:
                    float sourceValue = unit.GetStatByType(mod.sourceStat);
                    cross += sourceValue * mod.multiplier;
                    break;
            }
        }

        return Mathf.FloorToInt(flat + cross + (unit.GetStatByType(targetStat) * mult));
    }

    // returns just flat, useful for UI
    public int GetFlatBonus(StatType stat)
    {
        int sum = 0;
        foreach(var mod in modifiers)
        {
            if(mod.modType == StatModType.Flat && mod.targetStat == stat) sum += mod.flatValue;
        }
        return sum;
    }
}
