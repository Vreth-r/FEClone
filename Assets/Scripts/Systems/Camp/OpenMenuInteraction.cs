using UnityEngine;

[CreateAssetMenu(fileName = "NewOpenMenuInteraction", menuName = "Interactions/Open Menu")]
public class OpenMenuInteractionAction : InteractionAction
{
    public string menuID;

    public override void PerformAction()
    {
        base.PerformAction();
        switch (menuID)
        {
            case "ShopMenu":
                UIManagerTheSecond.Instance.OpenMenu<ShopMenuTheSecond>(menuID);
                break;
            case "InventoryManagerMenu":
                UIManagerTheSecond.Instance.OpenMenu<InventoryManagerMenu>(menuID);
                break;
        }
    }
}