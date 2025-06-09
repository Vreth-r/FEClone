using UnityEngine;

/// <summary>
/// Flat Stat Modify Effect:
/// Modifies a target unit's stat by a flat amount
/// Parameter scheme: 
/// string [stat], int [modifier], string [expire type (see effect.cs)]
/// </summary>
[CreateAssetMenu(menuName = "Tactics RPG/Effects/Flat Stat Mod")]
public class FlatStatModEffect : Effect
{
    public override void Apply(Unit source, Unit target, EffectContext context)
    {
        var p = context.parameters; // a EffectParameterMap
        foreach (var param in p.paramMap) // for every parameter in the map
        {
            target.statBonuses.AddModifier(new StatModifier(
                StatModType.Flat,
                GetStatTypeFromName(param.Key), 
                p.GetInt(param.Key),
                GetExpireTypeFromString(p.GetString(param.Key))));
        }
    }
}