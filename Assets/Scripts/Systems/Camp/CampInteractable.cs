using UnityEngine;

public class CampInteractable : MonoBehaviour
{
    [TextArea]
    public string interactText = "Press [E] to interact";
    public MenuType menuType; // set in editor

    public void Interact()
    {
        UIManager.Instance.OpenMenu(menuType);
        CampInputBlocker.SetBlocked(true);
    }
}