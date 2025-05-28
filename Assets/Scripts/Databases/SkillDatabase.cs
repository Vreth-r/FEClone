using UnityEngine;
using System.Collections.Generic;

// Skill Database, keeps a reference for each skill scriptable object

[CreateAssetMenu(menuName = "Tactics RPG/Skill Database")]
public class SkillDatabase : ScriptableObject
{
    public static SkillDatabase Instance; // singleton reference
    public List<Skill> skills; // List for editor population
    private Dictionary<string, Skill> skillLookup; // Dicitionary for O(1) reference

    private void OnEnable()
    {
        Instance = this; // assign singleton
        skillLookup = new Dictionary<string, Skill>(); // init dict

        foreach (var s in skills)
        {
            if (s != null && !skillLookup.ContainsKey(s.skillName)) // if item exists and not already in the database
            {
                skillLookup[s.skillName] = s; // add to DB
            }
        }
    }

    public static Skill GetSkillByName(string name)
    {
        if (Instance == null || Instance.skillLookup == null)
        {
            Debug.LogError("SkillDB not initialized");
            return null;
        }

        if (Instance.skillLookup.TryGetValue(name, out var result)) return result;

        Debug.LogWarning($"Skill {name} not found");
        return null;
    }
}