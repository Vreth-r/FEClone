using UnityEngine;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(menuName = "Units/Actions/Cancel Action")]
public class CancelAction : UnitAction
{
    public override async UniTask ExecuteAsync(Unit unit)
    {
        var move = unit.GetComponent<UnitMovement>();
        move.CancelMove();
        await UniTask.CompletedTask;
    }
}