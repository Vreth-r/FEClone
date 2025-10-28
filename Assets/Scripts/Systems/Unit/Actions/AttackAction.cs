using UnityEngine;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(menuName = "Units/Actions/Attack Action")]
public class AttackAction : UnitAction
{
    public override async UniTask ExecuteAsync(Unit unit)
    {
        unit.unitMovement.GetMovementRange().ClearHighlights();
        TargetSelector.Instance.BeginTargetingUnits(unit);
        await UniTask.Yield();
        TurnManager.Instance.TryEndPlayerTurn();
    }
}