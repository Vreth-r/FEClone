using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class UnitStatsUI : MonoBehaviour
{
    public TMP_Text unitNameText;
    public TMP_Text unitTitleText;

    public TMP_Text mhpText;
    public TMP_Text chpText;
    public TMP_Text strText;
    public TMP_Text arcText;
    public TMP_Text defText;
    public TMP_Text resText;
    public TMP_Text spdText;
    public TMP_Text sklText;
    public TMP_Text lckText;
    public TMP_Text avoText;
    public TMP_Text criText;
    public TMP_Text hitText;

    private Unit currentUnit;

    public void Show(Unit unit)
    {
        currentUnit = unit;
        unitNameText.text = unit.unitName;
        unitTitleText.text = unit.unitTitle;

        DisplayStat("MHP", unit.maxHP, unit.GetModifiedStat(unit.maxHP, "MHP"), mhpText);
        DisplayStat("CHP", unit.currentHP, unit.GetModifiedStat(unit.currentHP, "CHP"), chpText);
        DisplayStat("STR", unit.strength, unit.GetModifiedStat(unit.strength, "STR"), strText);
        DisplayStat("ARC", unit.arcane, unit.GetModifiedStat(unit.arcane, "ARC"), arcText);
        DisplayStat("DEF", unit.defense, unit.GetModifiedStat(unit.defense, "DEF"), defText);
        DisplayStat("SPD", unit.speed, unit.GetModifiedStat(unit.speed, "SPD"), spdText);
        DisplayStat("SKL", unit.skill, unit.GetModifiedStat(unit.skill, "SKL"), sklText);
        DisplayStat("RES", unit.resistance, unit.GetModifiedStat(unit.resistance, "RES"), resText);
        DisplayStat("LCK", unit.luck, unit.GetModifiedStat(unit.luck, "LCK"), lckText);
        DisplayStat("AVO", unit.avoidance, unit.GetModifiedStat(unit.avoidance, "AVO"), avoText);
        DisplayStat("CRI", unit.crit, unit.GetModifiedStat(unit.crit, "CRI"), criText);
        DisplayStat("HIT", unit.hit, unit.GetModifiedStat(unit.hit, "HIT"), hitText);
        gameObject.SetActive(true);
    }

    private void DisplayStat(string statName, int baseValue, int finalValue, TMP_Text displayText)
    {
        int bonus = finalValue - baseValue;

        StringBuilder sb = new();
        sb.Append($"{statName}: {baseValue}");

        if(bonus != 0)
        {
            string sign = bonus > 0 ? "+" : "-";
            sb.Append($"  ({sign}{Mathf.Abs(bonus)} bonus) = {finalValue}");
        }

        displayText.text = sb.ToString();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}