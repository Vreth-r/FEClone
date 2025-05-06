using UnityEngine;

// Effect to add bonus damage to a strike
/* 
Parameter scheme:
    string Mod : int [dmg]
*/

/* 
flat damage (deal +X extra damage) 
ignores def/res (ignore X% of enemy def/res) (?)
*/
[CreateAssetMenu(menuName = "Tactics RPG/Effects/Bonus Damage")]
public class BonusDamageEffect : Effect
{
    public override void Apply(Unit source, Unit target, EffectContext context)
    {
        var p = context.parameters;
        foreach (var param in p.paramMap)
        {
            if(param.Key == "Mod")
            {
                context.combat.bonusDamage += p.GetInt(param.Key);
            }
        }
    }
}