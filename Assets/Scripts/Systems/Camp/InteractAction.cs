using UnityEngine;

[CreateAssetMenu(fileName = "NewInteraction", menuName = "Interactions/Custom Interaction")]
public class InteractionAction : ScriptableObject
{
    public string actionName;
    public virtual void PerformAction()
    {
        Debug.Log($"{actionName} performed!");
    }
}