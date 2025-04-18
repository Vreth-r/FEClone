using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// will be expanded upon
// Skills are aspects of a unit that help them in combat
// ex: Sol from FE Awakening allows the unit to heal for 50% of the damage they deal from a strike, trigger chance was skill stat / 2

[CreateAssetMenu(fileName = "NewSkill", menuName = "Tactics RPG/Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
}
