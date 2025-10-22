using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Need to condense some of the UI shit but honestly it all might change when the actual final UI is designed so im holding off

public class CombatSceneManager : MonoBehaviour
{
    public CombatNarrator narrator;
    public CombatStatMenu attackerStats;
    public CombatStatMenu defenderStats;

    public GameObject uiObject;
    public HealthBarUI attackerHealthBar;
    public HealthBarUI defenderHealthBar;

    public float attackDelay = 1f;
    public float hitPause = 0.3f;
    public float cameraZoomDuration = 1f;
    public float cameraZoomTarget = 3f;
    public float cameraZoomDefault = 5f;

    public static CombatSceneManager Instance;

    // Text Fields
    public TextMeshProUGUI attackerName;
    public TextMeshProUGUI defenderName;
    public TextMeshProUGUI attackerHP;
    public TextMeshProUGUI defenderHP;
    public TextMeshProUGUI attackerInfo;
    public TextMeshProUGUI defenderInfo;
    public TextMeshProUGUI attackerWeaponName;
    public TextMeshProUGUI defenderWeaponName;
    public Image attackerWeaponIcon;
    public Image defenderWeaponIcon;

    private void Awake()
    {
        Instance = this; // singleton
    }

    public void EnterCombatScene(Unit attacker, Unit defender, CombatContext context, CombatQueue queue)
    {
        uiObject.SetActive(true);

        attackerName.text = attacker.unitName;
        defenderName.text = defender.unitName;

        attackerHP.text = $"HP: {context.attackerPrevHP}";
        defenderHP.text = $"HP: {context.defenderPrevHP}";

        attackerHealthBar.InstantFill(context.attackerPrevHP, attacker.maxHP);
        defenderHealthBar.InstantFill(context.defenderPrevHP, defender.maxHP);

        CombatPreviewHelper.GetCombatPreview(attacker, defender, context.attackerWeapon, out int baseA, out int bonusA, out int hitA, out int critA);
        CombatPreviewHelper.GetCombatPreview(defender, attacker, context.defenderWeapon, out int baseD, out int bonusD, out int hitD, out int critD);

        attackerInfo.text = CombatPreviewHelper.FormatCombatText(baseA, bonusA, hitA, critA);
        defenderInfo.text = CombatPreviewHelper.FormatCombatText(baseD, bonusD, hitD, critD);

        attackerStats.Open(attacker);
        defenderStats.Open(defender);

        attackerWeaponName.text = context.attackerWeapon.itemName;
        attackerWeaponIcon.sprite = context.attackerWeapon.icon;
        if (context.defenderWeapon != null)
        {
            defenderWeaponName.text = context.defenderWeapon.itemName;
            defenderWeaponIcon.sprite = context.defenderWeapon.icon;
        }
        else
        {
            defenderWeaponName.text = "None";
            // gotta make a placeholder sprite later
        }

        StartCoroutine(StartCombatSequence(attacker, defender, context, queue));
    }

    private IEnumerator StartCombatSequence(Unit attacker, Unit defender, CombatContext context, CombatQueue queue)
    {
        // cam setup
        CutsceneManager.Instance.cameraPanner.SetInCutscene(true);

        // pan to midpoint and zoom
        yield return StartCoroutine(CutsceneManager.Instance.cameraPanner.PanToLocation((attacker.transform.position + defender.transform.position) / 2f, 2f));
        yield return StartCoroutine(CutsceneManager.Instance.cameraPanner.ZoomCamera(cameraZoomTarget, 2f, 0.15f));

        // wait a second for cinema
        yield return new WaitForSeconds(0.5f);

        // begin fighting
        yield return StartCoroutine(PlayCombat(context, queue));

        // reset cam
        yield return StartCoroutine(CutsceneManager.Instance.cameraPanner.ZoomCamera(cameraZoomDefault, cameraZoomDuration, -1f));
        CutsceneManager.Instance.cameraPanner.SetInCutscene(false);
    }

    public void ExitCombat()
    {
        uiObject.SetActive(false);
    }

    public IEnumerator PlayCombat(CombatContext context, CombatQueue queue)
    {
        foreach (var action in queue.actions)
        {
            var attacker = action.attacker; // this is fucked up a bit but basically switches the attacker and defender based on which unit in the combat scene is attacking 
            var defender = action.defender;
            var attackerHPBar = attacker == context.attacker ? attackerHealthBar : defenderHealthBar;
            var defenderHPBar = defender == context.attacker ? attackerHealthBar : defenderHealthBar;
            var attackerHPText = attacker == context.attacker ? attackerHP : defenderHP;
            var defenderHPText = defender == context.attacker ? attackerHP : defenderHP;

            // Resolve attack
            CombatSystem.ResolveAttack(action, context);

            // Narration Line
            string message = $"{attacker.unitName} attacks!";
            if (action.isCounter) message += " (Counter)";
            if (action.isFollowUp) message += " (Follow-up)";
            yield return narrator.ShowMessageAndClear(message, 1f);

            // start anims
            attacker.animator?.SetTrigger("Attack");
            // the length property of AnimatorStateInfo gives the duration of the clip in seconds
            yield return new WaitForSeconds(attacker.animator.GetCurrentAnimatorStateInfo(0).length);
            if (action.attackerWeapon.visuals != null)
            {
                VFXManager.Instance.PlayEffect(action.attackerWeapon.visuals, attacker.gameObject.transform.position, defender.gameObject.transform.position);
            }

            // Capture HP before damage
            context.defenderPrevHP = defender.currentHP;

            // Show that shit
            if (context.hitting)
            {
                if (context.critting)
                {
                    // attacker crit visuals, maybe a light object or something i dunno think of this later
                    yield return narrator.ShowMessageAndClear("CRIT!", 1f);
                }
                else
                {
                    yield return narrator.ShowMessageAndClear("HIT!", 1f);
                }

                defender.animator?.SetTrigger("Hit");
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return narrator.ShowMessage("Miss!");
                defender.animator?.SetTrigger("Dodge");
                yield return new WaitForSeconds(defender.animator.GetCurrentAnimatorStateInfo(0).length);
            }

            // Update health bar and HP text
            defenderHPBar.SetHealth(defender.currentHP, defender.maxHP);
            defenderHPText.text = $"HP: {defender.currentHP}";

            // Death check
            if (defender.currentHP <= 0)
            {
                yield return narrator.ShowMessage($"{defender.unitName} was defeated!");
                // defender death here
                break; // cause he died
            }

            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(0.5f);
        ExitCombat();
    }
}
