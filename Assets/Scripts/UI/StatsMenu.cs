using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class StatsMenu : MonoBehaviour, IGameMenu
{
    public bool IsOpen { get; private set; }
    public MenuType MenuID => MenuType.StatMenu;
    public bool escapable { get; private set; }

    public GameObject background;

    [Header("Text References")]
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

    private Vector3 defaultPlacement = new Vector3(200, 125, 0);

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        escapable = false;
        IsOpen = false;
    }

    // Overload to keep interface from throwing a fit
    public void Open()
    {
        gameObject.SetActive(true);
        IsOpen = true;
    }

    public void Open(Unit unit)
    {
        currentUnit = unit;
        unitNameText.text = unit.unitName;
        unitTitleText.text = unit.unitTitle;

        background.transform.position = defaultPlacement;

        DisplayStat("MHP", unit.maxHP, unit.GetModifiedStat(StatType.MHP), mhpText);
        DisplayStat("CHP", unit.currentHP, unit.GetStatByType(StatType.CHP), chpText);
        DisplayStat("STR", unit.strength, unit.GetModifiedStat(StatType.STR), strText);
        DisplayStat("ARC", unit.arcane, unit.GetModifiedStat(StatType.ARC), arcText);
        DisplayStat("DEF", unit.defense, unit.GetModifiedStat(StatType.DEF), defText);
        DisplayStat("SPD", unit.speed, unit.GetModifiedStat(StatType.SPD), spdText);
        DisplayStat("SKL", unit.skill, unit.GetModifiedStat(StatType.SKL), sklText);
        DisplayStat("RES", unit.resistance, unit.GetModifiedStat(StatType.RES), resText);
        DisplayStat("LCK", unit.luck, unit.GetModifiedStat(StatType.LCK), lckText);
        DisplayStat("AVO", unit.avoidance, unit.GetModifiedStat(StatType.AVO), avoText);
        DisplayStat("CRI", unit.crit, unit.GetModifiedStat(StatType.CRI), criText);
        DisplayStat("HIT", unit.hit, unit.GetModifiedStat(StatType.HIT), hitText);
        Open();
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

    public void Close()
    {
        gameObject.SetActive(false);
        IsOpen = false;
    }
}