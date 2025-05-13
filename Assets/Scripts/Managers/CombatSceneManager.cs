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

    public void EnterCombatScene(Unit attacker, Unit defender, CombatContext context, CombatQueue queue)
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
    
        CombatPreviewHelper.GetCombatPreview(attacker, defender, context.attackerWeapon, out int baseA, out int bonusA, out int hitA, out int critA);
        CombatPreviewHelper.GetCombatPreview(defender, attacker, context.defenderWeapon, out int baseD, out int bonusD, out int hitD, out int critD);

        attackerInfo.text = CombatPreviewHelper.FormatCombatText(baseA, bonusA, hitA, critA);
        defenderInfo.text = CombatPreviewHelper.FormatCombatText(baseD, bonusD, hitD, critD);

        StartCoroutine(PlayCombat(context,  queue));
    }

    public void ExitCombat()
    {
        rootObject.SetActive(false);
        uiObject.SetActive(false);
        MouseTileHighlighter.Instance.enableFunction = true;
    }

    public IEnumerator PlayCombat(CombatContext context, CombatQueue queue)
    {
        foreach(var action in queue.actions)
        {
            var attacker = action.attacker; // this is fucked up a bit but basically switches the attacker and defender based on which unit in the combat scene is attacking 
            var defender = action.defender;
            var attackerView = attacker == context.attacker ? leftUnit : rightUnit;
            var defenderView = defender == context.attacker ? leftUnit : rightUnit;
            var attackerHPBar = attacker == context.attacker ? attackerHealthBar : defenderHealthBar;
            var defenderHPBar = defender == context.attacker ? attackerHealthBar : defenderHealthBar;
            var attackerHPText = attacker == context.attacker ? attackerHP : defenderHP;
            var defenderHPText = defender == context.attacker ? attackerHP : defenderHP;

            // Resolve attack
            CombatSystem.ResolveAttack(action, context);

            // Narration Line
            string message = $"{attacker.unitName} attacks!";
            if(action.isCounter) message += " (Counter)";
            if(action.isFollowUp) message += " (Follow-up)";
            yield return narrator.ShowMessageAndClear(message, 0.8f);

            if(attacker == leftUnit)
            {
                yield return attackerView.Lunge(1.0f);
            }
            else
            {
                yield return attackerView.Lunge(-1.0f);
            }
            yield return new WaitForSeconds(attackDelay);

            // Capture HP before damage
            context.defenderPrevHP = defender.currentHP;

            // Show that shit
            if (context.hitting)
            {
                if (context.critting)
                {
                    yield return attackerView.CritEffect();
                    yield return narrator.ShowMessageAndClear("CRIT!",  0.4f);
                }
                else
                {
                    yield return narrator.ShowMessageAndClear("HIT!");
                }

                yield return defenderView.FlashHit();
            }
            else
            {
                yield return narrator.ShowMessage("Miss!");
                yield return defenderView.Dodge();
            }

            // Update health bar and HP text
            defenderHPBar.SetHealth(defender.currentHP, defender.maxHP);
            defenderHPText.text = $"{defender.currentHP} / {defender.maxHP}";

            // Death check
            if (defender.currentHP <= 0)
            {
                yield return narrator.ShowMessage($"{defender.unitName} was defeated!");
                yield return defenderView.PlayDeath();
                break; // cause he died
            }

            yield return new WaitForSeconds(hitPause);
        }

        yield return new WaitForSeconds(0.5f);
        ExitCombat();
    }
}
