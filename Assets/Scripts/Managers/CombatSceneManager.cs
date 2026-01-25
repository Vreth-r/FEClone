using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

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

    CancellationTokenSource cts = new CancellationTokenSource();

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

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    public async UniTask EnterCombatSceneAsync(Unit attacker, Unit defender, CombatContext context, CombatQueue queue)
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

        attackerWeaponName.text = context.attackerWeapon.AsWeapon.itemName;
        attackerWeaponIcon.sprite = context.attackerWeapon.AsWeapon.icon;
        if (context.defenderWeapon != null)
        {
            defenderWeaponName.text = context.defenderWeapon.AsWeapon.itemName;
            defenderWeaponIcon.sprite = context.defenderWeapon.AsWeapon.icon;
        }
        else
        {
            defenderWeaponName.text = "None";
            // gotta make a placeholder sprite later
        }
        await StartCombatSequenceAsync(attacker, defender, context, queue);
    }

    private async UniTask StartCombatSequenceAsync(Unit attacker, Unit defender, CombatContext context, CombatQueue queue)
    {
        // cam setup
        CutsceneManager.Instance.cameraPanner.SetInCutscene(true);

        // pan to midpoint and zoom
        await CutsceneManager.Instance.cameraPanner.PanToLocationAsync((attacker.transform.position + defender.transform.position) / 2f, 2f);
        await CutsceneManager.Instance.cameraPanner.ZoomCameraAsync(cameraZoomTarget, 2f, 0.15f);

        // wait a second for cinema
        await UniTask.Delay(500);

        // begin fighting
        await PlayCombatAsync(context, queue, cts.Token);

        // reset cam
        await CutsceneManager.Instance.cameraPanner.ZoomCameraAsync(cameraZoomDefault, cameraZoomDuration, -1f);
        // death anims 
        if (attacker.currentHP == 0 && !context.attackerKilledSelf)
        {
            await attacker.Die(defender); // when these become cinematic (maybe) with a fade effect or something, await them
            await UniTask.Delay(500);
        }
        if(defender.currentHP == 0 && !context.defenderKilledSelf)
        {
            await defender.Die(attacker);
            await UniTask.Delay(500);
        }
        CutsceneManager.Instance.cameraPanner.SetInCutscene(false);
    }

    public void ExitCombat()
    {
        uiObject.SetActive(false);
    }

    public async UniTask PlayCombatAsync(CombatContext context, CombatQueue queue, CancellationToken cancellationToken)
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
            await narrator.ShowMessageAndClearAsync(message, 1f);

            // start anims
            attacker.animator?.SetTrigger("Attack");
            // the length property of AnimatorStateInfo gives the duration of the clip in seconds
            await UniTask.Delay((int)(attacker.animator.GetCurrentAnimatorStateInfo(0).length) * 1000);
            if (action.attackerWeapon.AsWeapon.visuals != null)
            {
                VFXManager.Instance.PlayEffect(action.attackerWeapon.AsWeapon.visuals, attacker.gameObject.transform.position, defender.gameObject.transform.position);
            }

            // Capture HP before damage
            context.defenderPrevHP = defender.currentHP;

            // Show that shit
            if (context.hitting)
            {
                if (context.critting)
                {
                    // attacker crit visuals, maybe a light object or something i dunno think of this later
                    await narrator.ShowMessageAndClearAsync("CRIT!", 1f);
                }
                else
                {
                    await narrator.ShowMessageAndClearAsync("HIT!", 1f);
                }

                defender.animator?.SetTrigger("Hit");
                await UniTask.Delay(500);
            }
            else
            {
                await narrator.ShowMessageAsync("Miss!");
                defender.animator?.SetTrigger("Dodge");
                await UniTask.Delay((int)(defender.animator.GetCurrentAnimatorStateInfo(0).length) * 1000);
            }

            // Update health bar and HP text
            defenderHPBar.SetHealth(defender.currentHP, defender.maxHP);
            defenderHPText.text = $"HP: {defender.currentHP}";

            // Death check
            if (defender.currentHP <= 0)
            {
                StatsAndAchievementManager.Instance.AddToStatistic(StatsAndAchievementManager.Stat.TOTAL_ENEMIES_DEFEATED, intData: (int)defender.team); // lol
                await narrator.ShowMessageAsync($"{defender.unitName} was defeated!");
                break; // this SHOULD be good enough for cancelling the rest of combat because of death but hey we'll see
            }

            await UniTask.Delay(1000);
        }

        await UniTask.Delay(500);
        ExitCombat();
    }
}
