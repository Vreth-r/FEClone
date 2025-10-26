using UnityEngine;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(menuName = "Units/Actions/Attack Action")]
public class AttackAction : UnitAction
{
    public override async UniTask ExecuteAsync(Unit unit)
    {
        TargetSelector.Instance.BeginTargeting(unit);
        await UniTask.Yield();
        TurnManager.Instance.TryEndPlayerTurn();
    }
}