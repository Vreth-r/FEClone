using UnityEngine;

public class CampShop : CampInteractable
{
    public override void Interact()
    {
        Debug.Log("Opening Shop...");
        CampUIManager.Instance.OpenMenu(CampMenuType.Shop);
    }
}