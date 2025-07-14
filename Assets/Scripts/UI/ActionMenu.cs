using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Gives functionality to action menu ui buttons

public class ActionMenu : NavMenu
{
    public Button attackButton; //  button references
    public Button waitButton;
    public Button itemButton;
    public Button cancelButton;
    public GameObject background;

    private UnitMovement activeUnit; // the unit in question:

    public override MenuType MenuID => MenuType.ActionMenu;

    public override void Open()
    {
        base.Open();
    }

    public void Open(UnitMovement unit, Vector3 worldPos) // overload
    {
        activeUnit = unit; // set the active unit for later
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos + new Vector3(1f, 1.5f, 0)); // get world to screen coords
        background.transform.position = screenPos; // set position

        menuButtons = new List<Button> { attackButton, waitButton, itemButton, cancelButton };

        // button callbacks
        /*
        attackButton.onClick.RemoveAllListeners();
        attackButton.onClick.AddListener(OnAttack);

        waitButton.onClick.RemoveAllListeners();
        waitButton.onClick.AddListener(OnWait);

        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(OnItem);

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(OnCancel);
        */

        Open();
    }

    private void OnAttack()
    {
        Debug.Log("Attack!");
        activeUnit.OnMenuSelect(UnitActionType.Attack);
        UIManager.Instance.CloseTopMenu();
    }

    private void OnWait()
    {
        Debug.Log("Wait.");
        activeUnit.OnMenuSelect(UnitActionType.Wait);
        UIManager.Instance.CloseTopMenu();
    }

    private void OnItem()
    {
        Debug.Log("Use Item.");
        activeUnit.OnMenuSelect(UnitActionType.Item);
        UIManager.Instance.CloseTopMenu();
    }

    private void OnCancel()
    {
        Debug.Log("Cancel move.");
        UIManager.Instance.CloseTopMenu();
        activeUnit.OnMenuSelect(UnitActionType.Cancel);
    }
}

public enum UnitActionType { Attack, Wait, Item, Cancel }
