using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string interactableName = "Interactable Object";  // balls
    public string interactionText = "Press E to interact";
    public InteractionAction interactAction;


    // trigger interaction
    public void Interact()
    {
        interactAction.PerformAction();
    }
}
