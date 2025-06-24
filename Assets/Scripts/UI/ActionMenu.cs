using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Gives functionality to action menu ui buttons

public class ActionMenu : MonoBehaviour, IGameMenu
{
    public Button attackButton; //  button references
    public Button waitButton;
    public Button itemButton;
    public Button cancelButton;
    public GameObject background;
    public bool IsOpen { get; private set; } // i know about gameObject.activeSelf but i need the ref somewhere else for the game object and only the script is passed to UnitMovement\
    public MenuType MenuID => MenuType.ActionMenu;
    public bool escapable { get; private set; }

    private UnitMovement activeUnit; // the unit in question:

    public void Awake()
    {
        attackButton.onClick.AddListener(OnAttack);
        waitButton.onClick.AddListener(OnWait);
        itemButton.onClick.AddListener(OnItem);
        cancelButton.onClick.AddListener(OnCancel);
        escapable = false;
        IsOpen = false;
    }
    
    public void Open() // will be overloaded to make the interface happy
    {
        IsOpen = true;
        gameObject.SetActive(true);
    }

    public void Open(UnitMovement unit, Vector3 worldPos) // overload
    {
        Debug.Log("Action Menu With Unit");
        MouseTileHighlighter.Instance.enableFunction = false; // dont move camera when action menu is up
        activeUnit = unit; // set the active unit for later
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos + new Vector3(1f, 1.5f, 0)); // get world to screen coords
        background.transform.position = screenPos; // set position
        Open();
    }

    public void Close()
    {
        MouseTileHighlighter.Instance.enableFunction = true; // reactivate camera movement
        IsOpen = false;
        gameObject.SetActive(false);
        if (UIManager.Instance.GetCurrentMenuType() == MenuID)
        {
            UIManager.Instance.WipeCurrentMenu();
        }
    }

    private void OnAttack()
    {
        Debug.Log("Attack!");
        activeUnit.OnMenuSelect(UnitActionType.Attack);
        Close();
    }

    private void OnWait()
    {
        Debug.Log("Wait.");
        activeUnit.OnMenuSelect(UnitActionType.Wait);
        Close();
    }

    private void OnItem()
    {
        Debug.Log("Use Item.");
        activeUnit.OnMenuSelect(UnitActionType.Item);
        Close();
    }

    private void OnCancel()
    {
        Debug.Log("Cancel move.");
        Close();
        activeUnit.OnMenuSelect(UnitActionType.Cancel);
    }
}

public enum UnitActionType { Attack, Wait, Item, Cancel }
