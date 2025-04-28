using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Simple effect to add a crossstatmod to a unit (Like ARC = MAXHP/2)\
/*
Parameter scheme: 
string Target : string [Stat]
string Source : string [Stat]
string Mod : float [mod]
*/

[CreateAssetMenu(menuName = "Tactics RPG/Effects/Cross Stat Mod")]
public class CrossStatModEffect : Effect
{
    private List<string> targetStats = new();
    private List<string> sourceStats = new();
    private List<float> mods = new();
    public override void Apply(Unit source, Unit target, EffectContext context)
    {
        var p = context.parameters; // grab the paramMap
        foreach (var param in p.paramMap) // for every parameter in the map
        {   
            if (param.Key == "Target")
            {
                targetStats.Add(p.GetString(param.Key)); // If its a target parameter, add it to the targets stats list
            } else if (param.Key == "Source")
            {
                sourceStats.Add(p.GetString(param.Key)); // etc
            } else 
            {
                mods.Add(p.GetFloat(param.Key));
            }
        }

        // Add the cross stat mods respectively
        for (int i = 0; i < targetStats.Count; i++)
        {
            target.statBonuses.AddCrossStatMod(GetStatTypeFromName(targetStats[i]), GetStatTypeFromName(sourceStats[i]), mods[i]);
        }
    }
}