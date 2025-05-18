using UnityEngine;

public abstract class CampInteractable : MonoBehaviour
{
    [TextArea]
    public string interactText = "Press [E] to interact";

    public abstract void Interact();
}