using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatSceneManager : MonoBehaviour
{
    public CombatUnitView leftUnit;
    public CombatUnitView rightUnit;
    public CombatNarrator narrator;
    public GameObject dimmerOverlay;

    public GameObject rootObject; // the canvas root to activate/deactivate
    public GameObject uiObject;

    public float attackDelay = 0.8f;
    public float hitPause = 0.3f;

    public static CombatSceneManager Instance;

    private void Awake() => Instance = this; // declare this instance for external ref

    public void EnterCombatScene(Unit attacker, Unit defender, CombatContext context)
    {
        MouseTileHighlighter.Instance.enableFunction = false;
        rootObject.SetActive(true); // can also be faded in later
        uiObject.SetActive(true);

        leftUnit.SetFromUnit(attacker, true);
        rightUnit.SetFromUnit(defender, false);

        leftUnit.FlipSprite(true);

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
        yield return narrator.ShowMessageAndClear($"{context.attacker.unitName} attacks!");
        yield return leftUnit.Lunge();

        yield return new WaitForSeconds(attackDelay);

        if (context.hitting)
        {
            yield return narrator.ShowMessage("HIT!");
            yield return rightUnit.FlashHit();

            if(context.critting)
            {
                yield return narrator.ShowMessage("CRIT!");
                yield return leftUnit.CritEffect();
            }

            yield return new WaitForSeconds(hitPause);

            if(context.defender.currentHP <= 0)
            {
                yield return narrator.ShowMessage($"{context.defender.unitName} was defeated!");
                yield return rightUnit.PlayDeath();
            }
        }
        else
        {
            yield return narrator.ShowMessage("Miss!");
            yield return rightUnit.Dodge();
        }

        yield return new WaitForSeconds(1f); // buffer
        ExitCombat();
    }
}
