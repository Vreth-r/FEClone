using UnityEngine;

public class CampInteractable : MonoBehaviour
{
    [TextArea]
    public string interactText = "Press [E] to interact";
    public MenuType menuType; // change this later to support new UI system

    public virtual void Interact()
    {   
        UIManagerTheSecond.Instance.OpenMenu<ShopMenuTheSecond>("ShopMenu");
        CampInputBlocker.SetBlocked(true);
    }
}