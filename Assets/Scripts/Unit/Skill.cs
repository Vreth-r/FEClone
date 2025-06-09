using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Skills are aspects of a unit that help them in combat
// ex: Sol from FE Awakening allows the unit to heal for 50% of the damage they deal from a strike, trigger chance was skill stat / 2

// In this game, skills are containers for really cool effects
[CreateAssetMenu(menuName = "Tactics RPG/Skill")]
public class Skill : ScriptableObject, IIdentifiable
{
    public string skillName;
    [TextArea] public string description;
    public string ID => skillName;
    public Sprite icon;
    public List<EffectInstance> effects;

    public void ProcEffects(Unit source, Unit target, Event evnt, EffectContext context = null)
    {
        foreach (var e in effects)
        {
            foreach (var triggerData in e.triggerConditions)
            {
                if (triggerData.evnt != evnt) continue; // skip it if its event isnt being proced

                if (context == null) context = new EffectContext(); // create context if there isnt already

                if (!triggerData.AreConditionsMet(source, target, context)) continue; // check conditions

                if (triggerData.evnt == Event.Passive)
                {
                    // always apply passives
                    e.Apply(source, target, context);
                }
                else
                {
                    int chance = Mathf.FloorToInt(triggerData.GetProcChance(source));
                    if (source.Roll(chance))
                    {
                        Debug.Log($"Skill [{skillName}] triggered by {source.unitName}! Chance: {chance:F1}%");
                        e.Apply(source, target, context);
                    }
                }
            }
        }
    }
}

