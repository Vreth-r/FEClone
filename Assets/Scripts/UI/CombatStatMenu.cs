using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

// This cant be a IGameMenu implement because its only used in the combat scene manager script and doesnt follow normal UI rules
// might refactor this to have ALL info for a unit in combat and just have the manager tell this to run at certain times, just to condense,
// but for now i wrote this as a debug tool really (im not firing up a debugger every 2 seconds and we might use this for final product anyway)
public class CombatStatMenu : MonoBehaviour
{
    public TMP_Text strText;
    public TMP_Text arcText;
    public TMP_Text defText;
    public TMP_Text resText;
    public TMP_Text spdText;
    public TMP_Text sklText;
    public TMP_Text lckText;

    // i copied this all from StatsMenu so ill make it better later when the UI is finalized
    public void Open(Unit unit)
    {
        DisplayStat("STR", unit.strength, unit.GetModifiedStat(StatType.STR), strText);
        DisplayStat("ARC", unit.arcane, unit.GetModifiedStat(StatType.ARC), arcText);
        DisplayStat("DEF", unit.defense, unit.GetModifiedStat(StatType.DEF), defText);
        DisplayStat("SPD", unit.speed, unit.GetModifiedStat(StatType.SPD), spdText);
        DisplayStat("SKL", unit.skill, unit.GetModifiedStat(StatType.SKL), sklText);
        DisplayStat("RES", unit.resistance, unit.GetModifiedStat(StatType.RES), resText);
        DisplayStat("LCK", unit.luck, unit.GetModifiedStat(StatType.LCK), lckText);
    }

    private void DisplayStat(string statName, int baseValue, int finalValue, TMP_Text displayText)
    {
        displayText.text = $"{statName}: {baseValue} | {finalValue}";
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}