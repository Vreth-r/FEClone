using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// designed to work both as a prof tracker for units and a stat container for classes and items

[System.Serializable]
public class WeaponProficiency
{
    [SerializeField] private List<WeaponProficiencyEntry> profList = new List<WeaponProficiencyEntry>();

    private Dictionary<WeaponType, int> runtimeProfDict;

    private void BuildRuntimeDictionary()
    {
        runtimeProfDict = new Dictionary<WeaponType, int>();
        foreach (var entry in profList)
        {
            runtimeProfDict[entry.weaponType] = entry.level;
        }
    }

    public void Initialize()
    {
        if (runtimeProfDict == null) BuildRuntimeDictionary();
    }

    public bool HasProficiency(WeaponType weapon)
    {
        return runtimeProfDict.ContainsKey(weapon);
    }

    public void AddProficiency(WeaponType weapon, int level = 0)
    {
        if(runtimeProfDict.ContainsKey(weapon)) return;
        profList.Add(new WeaponProficiencyEntry { weaponType = weapon, level = level });
        runtimeProfDict[weapon] = level;
    }

    public bool CheckWeapon(WeaponItem weapon)
    {
        if(weapon.proficiency.GetProficiencies() == null)
        {
            weapon.proficiency.Initialize();
        }
        foreach (var proficiency in weapon.proficiency.GetProficiencies())
        {
            if(this.runtimeProfDict.ContainsKey(proficiency.Key) && (this.runtimeProfDict[proficiency.Key] >= proficiency.Value))
            {
                return true;
            }
        }
        return false;
    }

    public void AddProficienciesFromOther(WeaponProficiency proficiencies)
    {
        foreach (var proficiency in proficiencies.GetProficiencies()) // for every allowed weapon
        {  
            this.AddProficiency(proficiency.Key);
        }
    }

    public void RemoveProficiency(WeaponType weapon)
    {
        if(!runtimeProfDict.ContainsKey(weapon)) return;

        profList.RemoveAll(e => e.weaponType == weapon);
        runtimeProfDict.Remove(weapon);
    }

    public int GetProficiencyAmount(WeaponType weapon)
    {
        if(!runtimeProfDict.ContainsKey(weapon)) return -1;
        return runtimeProfDict[weapon];
    }

    public string GetProficiencyLetter(WeaponType weapon)
    {
        if(!runtimeProfDict.ContainsKey(weapon)) return "-";
        int prof = runtimeProfDict[weapon];
        if(prof <= 9 && prof >= 0) return "D";
        if(prof <= 19) return "C";
        if(prof <= 29) return "B";
        if(prof <= 39) return "A";
        if(prof == 40) return "S";
        return "";
    }

    public void RaiseProficiency(WeaponType weapon)
    {
        if(!runtimeProfDict.ContainsKey(weapon)) return;
        if(runtimeProfDict[weapon] < 40)
        {
            int index = profList.FindIndex(e => e.weaponType == weapon);
            if (index >= 0) profList[index].level += 1;
            runtimeProfDict[weapon] += 1;
        }
    }

    public Dictionary<WeaponType, int> GetProficiencies()
    {
        return runtimeProfDict;
    }

    public void Clear()
    {
        profList.Clear();
        runtimeProfDict.Clear();
    }
}

[System.Serializable]
public class WeaponProficiencyEntry
{
    public WeaponType weaponType;
    public int level;
}