using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatSceneManager : MonoBehaviour
{
    public CombatUnitView leftUnit;
    public CombatUnitView rightUnit;
    public CombatNarrator narrator;
    public GameObject dimmerOverlay;

    public GameObject rootObject; // the canvas root to activate/deactivate
    public GameObject uiObject;

    public HealthBarUI attackerHealthBar;
    public HealthBarUI defenderHealthBar;

    public float attackDelay = 0.8f;
    public float hitPause = 0.3f;

    public static CombatSceneManager Instance;

    // Text Fields
    public TextMeshProUGUI attackerName;
    public TextMeshProUGUI defenderName;
    public TextMeshProUGUI attackerHP;
    public TextMeshProUGUI defenderHP;
    public TextMeshProUGUI attackerInfo;
    public TextMeshProUGUI defenderInfo;

    private void Awake() => Instance = this; // declare this instance for external ref

    public void EnterCombatScene(Unit attacker, Unit defender, CombatContext context)
    {
        MouseTileHighlighter.Instance.enableFunction = false; //  disable the tile highlights when the combat scene is running
        rootObject.SetActive(true); // can also be faded in later
        uiObject.SetActive(true);

        leftUnit.SetFromUnit(attacker, true);
        rightUnit.SetFromUnit(defender, false);

        leftUnit.FlipSprite(true);

        attackerName.text = attacker.unitName;
        defenderName.text = defender.unitName;

        attackerHP.text = $"{context.attackerPrevHP} / {attacker.maxHP}";
        defenderHP.text = $"{context.defenderPrevHP} / {defender.maxHP}";

        attackerHealthBar.InstantFill(context.attackerPrevHP, attacker.maxHP);
        defenderHealthBar.InstantFill(context.defenderPrevHP, defender.maxHP);

        attackerInfo.text = $"{context.baseDamage}\nHit: {context.hitChance}%\nCrit: {context.critChance}%";

        StartCoroutine(PlayCombat(context));
    }

    public void ExitCombat()
    {
        rootObject.SetActive(false);
        uiObject.SetActive(false);
        MouseTileHighlighter.Instance.enableFunction = true;
    }

    public IEnumerator PlayCombat(CombatContext context)
    {
        // show initial attack message
        yield return narrator.ShowMessageAndClear($"{context.attacker.unitName} attacks!", 0.7f);

        yield return new WaitForSeconds(0.5f);

        if (context.hitting)
        {
            if(context.critting)
            {
                yield return leftUnit.CritEffect();
                yield return new WaitForSeconds(0.5f);
                yield return leftUnit.Lunge(); // attacking unit visuals
                yield return narrator.ShowMessage("CRIT!");
                yield return new WaitForSeconds(attackDelay);
            }
            else
            {
                yield return leftUnit.Lunge(); // attacking unit visuals
                yield return new WaitForSeconds(attackDelay);
                yield return narrator.ShowMessage("HIT!");
                yield return rightUnit.FlashHit();
            }
            defenderHealthBar.SetHealth(context.defender.currentHP, context.defender.maxHP);
            defenderHP.text = $"{context.defender.currentHP} / {context.defender.maxHP}";
            yield return new WaitForSeconds(hitPause);

            if(context.defender.currentHP <= 0)
            {
                yield return narrator.ShowMessage($"{context.defender.unitName} was defeated!");
                yield return rightUnit.PlayDeath();
            }
        }
        else
        {
            yield return leftUnit.Lunge();
            yield return new WaitForSeconds(attackDelay);
            yield return narrator.ShowMessage("Miss!");
            yield return rightUnit.Dodge();
        }

        yield return new WaitForSeconds(1f); // buffer
        ExitCombat();
    }
}
