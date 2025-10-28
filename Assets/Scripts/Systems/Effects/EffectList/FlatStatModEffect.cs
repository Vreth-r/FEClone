using UnityEngine;

/// <summary>
/// Flat Stat Modify Effect:
/// Modifies a target unit's stat by a flat amount
/// Parameter scheme: 
/// KEY: string [stat] | VALUE: int [modifier], string [string [expire type (see effect.cs)]|int [turn count]]
/// example: KEY: HIT | VALUE: 10, "TURN|1"
/// </summary>
[CreateAssetMenu(menuName = "Tactics RPG/Effects/Flat Stat Mod")]
public class FlatStatModEffect : Effect
{
    public override void Apply(Unit source, Unit target, EffectContext context)
    {
        var p = context.parameters; // a EffectParameterMap
        foreach (var param in p.paramMap) // for every parameter in the map
        {
            string[] expireData = p.GetString(param.Key).Split('|');
            target.statBonuses.AddModifier(new StatModifier(
                StatModType.Flat,
                GetStatTypeFromName(param.Key),
                p.GetInt(param.Key),
                GetExpireTypeFromString(expireData[0]),
                int.Parse(expireData[1])
            ));
        }
    }
}