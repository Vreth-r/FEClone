using UnityEngine;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(menuName = "Units/Actions/Unit Action")]
public abstract class UnitAction : ScriptableObject
{
    [Header("UI")]
    public string actionName = "Action";
    public Sprite icon;

    [Header("Conditions")]
    public bool requiresTarget = false;

    public virtual bool IsAvailable(Unit unit) => true;

    public abstract UniTask ExecuteAsync(Unit unit); // might as well make this async
}