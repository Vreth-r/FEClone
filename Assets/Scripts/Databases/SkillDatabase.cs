using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tactics RPG/Skill Database")]
public class SkillDatabase : ScriptableObject
{
    public static SkillDatabase Instance;
    public List<Skill> skills;
    private Dictionary<string, Skill> skillLookup;

    private void OnEnable()
    {
        Instance = this;
        skillLookup = new Dictionary<string, Skill>();

        foreach (var s in skills)
        {
            if (s != null && !skillLookup.ContainsKey(s.skillName))
            {
                skillLookup[s.skillName] = s;
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