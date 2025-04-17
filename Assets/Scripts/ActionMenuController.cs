using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Gives functionality to action menu ui buttons

public class ActionMenuController : MonoBehaviour
{
    public Button attackButton; //  button references
    public Button waitButton;
    public Button itemButton;
    public Button cancelButton;

    private UnitMovement activeUnit; // the unit in question:

    public void Show(UnitMovement unit, Vector3 worldPos)
    {
        activeUnit = unit; // set the active unit for later
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos + new Vector3(1f, 1.5f, 0)); // get world to screen coords
        transform.position = screenPos; // set position
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Start()
    {
        attackButton.onClick.AddListener(OnAttack);
        waitButton.onClick.AddListener(OnWait);
        itemButton.onClick.AddListener(OnItem);
        cancelButton.onClick.AddListener(OnCancel);
    }

    private void OnAttack()
    {
        Debug.Log("Attack!");
        activeUnit.OnMenuSelect(UnitActionType.Attack);
        Hide();
    }

    private void OnWait()
    {
        Debug.Log("Wait.");
        activeUnit.OnMenuSelect(UnitActionType.Wait);
        Hide();
    }

    private void OnItem()
    {
        Debug.Log("Use Item.");
        activeUnit.OnMenuSelect(UnitActionType.Item);
        Hide();
    }

    private void OnCancel()
    {
        Debug.Log("Cancel move.");
        activeUnit.OnMenuSelect(UnitActionType.Cancel);
        Hide();
    }
}

public enum UnitActionType { Attack, Wait, Item, Cancel }
