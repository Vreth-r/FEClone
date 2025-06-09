using UnityEngine;
using System.Collections.Generic;

// Skill Database, keeps a reference for each skill scriptable object

[CreateAssetMenu(menuName = "Tactics RPG/Skill Database")]
public class SkillDatabase : Database<Skill>
{
    public static SkillDatabase Instance; // singleton reference

    public void Init()
    {
        base.Initialize(); // Init db
        if (Instance == null) Instance = this;
    }
}