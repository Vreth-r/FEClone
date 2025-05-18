using UnityEngine;

public class CampBarracks : CampInteractable
{
    public override void Interact()
    {
        Debug.Log("Opening Barracks...");
        CampUIManager.Instance.OpenMenu(CampMenuType.Army);
    }
}