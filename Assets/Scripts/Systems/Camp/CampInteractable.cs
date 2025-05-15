using UnityEngine;

public abstract class CampInteractable : MonoBehaviour
{
    [TextArea]
    public string interactText = "Press [E] to interact";

    public abstract void Interact();
}

public class CampBarracks : CampInteractable
{
    public override void Interact()
    {
        CampUIManager.Instance.OpenMenu(CampMenuType.Army);
    }
}