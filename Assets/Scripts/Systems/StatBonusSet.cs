using System.Collections;
using System.Collections.Generic;
public class StatBonusSet
{
    // Class to keep track of stat bonuses for units
    // used for passive skills and weapons 
    public Dictionary<StatType, int> flatMods = new();

    public Dictionary<StatType, float> multMods = new();

    // for use for skills modifying stats based off other or the same stats
    public List<CrossStatMultMod> crossStatModifiers;

    public StatBonusSet()
    {
        flatMods[StatType.MHP] = 0;
        flatMods[StatType.STR] = 0;
        flatMods[StatType.ARC] = 0;
        flatMods[StatType.DEF] = 0;
        flatMods[StatType.SPD] = 0;
        flatMods[StatType.SKL] = 0;
        flatMods[StatType.RES] = 0;
        flatMods[StatType.LCK] = 0;
        flatMods[StatType.AVO] = 0;
        flatMods[StatType.CRI] = 0;
        flatMods[StatType.HIT] = 0;

        // Range: 0 - 1 due to how the stat is calculated
        multMods[StatType.MHP] = 0;
        multMods[StatType.STR] = 0;
        multMods[StatType.ARC] = 0;
        multMods[StatType.DEF] = 0;
        multMods[StatType.SPD] = 0;
        multMods[StatType.SKL] = 0;
        multMods[StatType.RES] = 0;
        multMods[StatType.LCK] = 0;
        multMods[StatType.AVO] = 0;
        multMods[StatType.CRI] = 0;
        multMods[StatType.HIT] = 0;
        crossStatModifiers = new();
    }

    public void AddFlatStatMod(StatType stat, int mod)
    {
        flatMods[stat] += mod;
    }

    public void RemoveFlatStatMod(StatType stat, int mod)
    {
        flatMods[stat] -= mod;
    }

    public int GetFlatMod(StatType stat)
    {
        return flatMods[stat];
    }

    public void AddMultStatMod(StatType stat, float mod)
    {
        multMods[stat] += mod;
    }

    public void RemoveMultStatMod(StatType stat, float mod)
    {
        multMods[stat] -= mod;
    }

    public float GetMultMod(StatType stat)
    {
        return multMods[stat];
    }

    public void AddCrossStatMod(StatType targetStat, StatType sourceStat, float mod)
    {
        crossStatModifiers.Add(new CrossStatMultMod(targetStat, sourceStat, mod));
    }

    public void RemoveCrossStatMod(StatType targetStat, StatType sourceStat, float modS)
    {
        foreach (CrossStatMultMod mod in crossStatModifiers)
        {
            if (mod.targetStat == targetStat && mod.sourceStat == sourceStat && mod.multiplier == modS)
            {
                crossStatModifiers.Remove(mod);
            }
        }
    }

    public void Clear()
    {
        flatMods[StatType.MHP] = 0;
        flatMods[StatType.STR] = 0;
        flatMods[StatType.ARC] = 0;
        flatMods[StatType.DEF] = 0;
        flatMods[StatType.SPD] = 0;
        flatMods[StatType.SKL] = 0;
        flatMods[StatType.RES] = 0;
        flatMods[StatType.LCK] = 0;
        flatMods[StatType.AVO] = 0;
        flatMods[StatType.CRI] = 0;
        flatMods[StatType.HIT] = 0;

        // Range: 0 - 1 due to how the stat is calculated
        multMods[StatType.MHP] = 0;
        multMods[StatType.STR] = 0;
        multMods[StatType.ARC] = 0;
        multMods[StatType.DEF] = 0;
        multMods[StatType.SPD] = 0;
        multMods[StatType.SKL] = 0;
        multMods[StatType.RES] = 0;
        multMods[StatType.LCK] = 0;
        multMods[StatType.AVO] = 0;
        multMods[StatType.CRI] = 0;
        multMods[StatType.HIT] = 0;

        crossStatModifiers.Clear();
    }
}
