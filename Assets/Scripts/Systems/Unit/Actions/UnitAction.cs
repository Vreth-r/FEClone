using UnityEngine;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(menuName = "Units/Actions/Unit Action")]
public abstract class UnitAction : ScriptableObject
{
    [Header("UI")]
    public string actionName = "Action";
    public Color color = Color.black;
    public Sprite icon;

    [Header("Conditions")]
    public bool requiresTarget = false;

    [Header("Cooldown")]
    public bool onCooldown = false;
    public int cooldown = 0; // total turns to wait
    private int remainingCooldown = 0; // internal cooldown

    public virtual bool IsAvailable(Unit unit)
    {
        return !onCooldown;
    }

    public async UniTask TryExecuteAsync(Unit unit)
    {
        if (onCooldown) return;

        await ExecuteAsync(unit);

        if (cooldown > 0)
        {
            StartCooldown();
        }
    }

    public abstract UniTask ExecuteAsync(Unit unit); // might as well make this async

    private void StartCooldown()
    {
        onCooldown = true;
        remainingCooldown = cooldown;
        TurnManager.Instance.OnTurnFlip += HandleTurnFlip;
    }

    private void HandleTurnFlip(int turnNumber)
    {
        if (!onCooldown) return;

        remainingCooldown--;
        if (remainingCooldown <= 0)
        {
            onCooldown = false;
            remainingCooldown = 0;
            TurnManager.Instance.OnTurnFlip -= HandleTurnFlip;
        }
    }

    public int GetRemainingCooldown() => remainingCooldown;
}