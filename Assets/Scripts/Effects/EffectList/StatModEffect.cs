using UnityEngine;

[CreateAssetMenu(menuName = "Tactics RPG/Effects/Stat Mod")]
public class StatModEffect : Effect
{
    public override void Apply(Unit source, Unit target, EffectContext context)
    {
        var p = context.parameters;
        target.statBonuses.AddFlatStatMod(StatType.STR, p.Get("STR"));
        target.statBonuses.AddFlatStatMod(StatType.ARC, p.Get("ARC"));
        target.statBonuses.AddFlatStatMod(StatType.DEF, p.Get("DEF"));
        target.statBonuses.AddFlatStatMod(StatType.SPD, p.Get("SPD"));
        target.statBonuses.AddFlatStatMod(StatType.SKL, p.Get("SKL"));
        target.statBonuses.AddFlatStatMod(StatType.RES, p.Get("RES"));
        target.statBonuses.AddFlatStatMod(StatType.LCK, p.Get("LCK"));
        target.statBonuses.AddFlatStatMod(StatType.AVO, p.Get("AVO"));
        target.statBonuses.AddFlatStatMod(StatType.HIT, p.Get("HIT"));
        target.statBonuses.AddFlatStatMod(StatType.CRI, p.Get("CRI"));
    }
}