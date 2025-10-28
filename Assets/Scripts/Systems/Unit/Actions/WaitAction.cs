using UnityEngine;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(menuName = "Units/Actions/Wait Action")]
public class WaitAction : UnitAction
{
    public override async UniTask ExecuteAsync(Unit unit)
    {
        unit.unitMovement.GetMovementRange().ClearHighlights();
        unit.state = UnitState.Tapped;
        unit.GetComponent<UnitMovement>().enabled = false;
        TurnManager.Instance.TryEndPlayerTurn();
        await UniTask.CompletedTask;
    }
}