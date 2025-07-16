using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class NavMenu : MonoBehaviour, IGameMenu
{
    protected List<Button> menuButtons = new();
    protected int selectedIndex = 0;
    protected float inputCooldown = 0.2f;
    private float lastInputTime;

    [SerializeField] private RectTransform selectionIndicator; // assing in inspector
    [SerializeField] private Vector3 indicatorOffset = new Vector3(-20f, 0f, 0f);

    public bool IsOpen { get; protected set; }
    public abstract MenuType MenuID { get; }
    public virtual bool escapable => true;

    protected virtual void OnEnable()
    {
        ControlsManager.Instance.OnSelect += HandleSelect;
        ControlsManager.Instance.OnCancel += HandleCancel;
    }

    protected virtual void OnDisable()
    {
        ControlsManager.Instance.OnSelect -= HandleSelect;
        ControlsManager.Instance.OnCancel -= HandleCancel;
    }

    public virtual void Open()
    {
        IsOpen = true;
        gameObject.SetActive(true);
        selectedIndex = 0;
        HighlightButton(selectedIndex);
        SetButtonsInteractable(true);
        Debug.Log("Opening Nav Menu");
    }

    public virtual void Close()
    {
        IsOpen = false;
        gameObject.SetActive(false);
        SetButtonsInteractable(false);
        UIManager.Instance.WipeCurrentMenu();
        if (selectionIndicator != null)
            selectionIndicator.gameObject.SetActive(false);
    }

    public virtual void FixedUpdate()
    {
        if (!IsOpen) return;

        Vector2 nav = ControlsManager.Instance.NavigateInput;
        if (Time.time - lastInputTime < inputCooldown || nav == Vector2.zero) return;

        if (Mathf.Abs(nav.y) > Mathf.Abs(nav.x))
        {
            int dir = (int)Mathf.Sign(-nav.y);
            selectedIndex += dir;

            if (selectedIndex < 0) selectedIndex = menuButtons.Count - 1;
            if (selectedIndex >= menuButtons.Count) selectedIndex = 0;

            HighlightButton(selectedIndex);
            lastInputTime = Time.time;
        }
    }

    void LateUpdate()
    {
        if (IsOpen && selectionIndicator != null && selectedIndex >= 0)
        {
            var target = menuButtons[selectedIndex].GetComponent<RectTransform>();
            Vector3 desiredPosition = target.position + indicatorOffset;
            selectionIndicator.position = Vector3.Lerp(selectionIndicator.position, desiredPosition, 10f * Time.deltaTime);
        }
    }


    protected virtual void HighlightButton(int index)
    {
        if (index >= 0 && index < menuButtons.Count)
        {
            var target = menuButtons[index].GetComponent<RectTransform>();
            //menuButtons[index].Select();

            if (selectionIndicator != null && target != null)
            {
                selectionIndicator.gameObject.SetActive(true);
                selectionIndicator.position = target.position + indicatorOffset;
            }
        }
    }

    protected virtual void HandleSelect()
    {
        Debug.Log("NavMenu: HandleSelect Proc");
        if (!IsOpen || ControlsManager.Instance.CurrentContext != InputContext.Menu) return;
        Debug.Log("Invoking a button");
        menuButtons[selectedIndex].onClick.Invoke(); // this will fire its invocation regardless of the buttons active state
    }

    protected virtual void HandleCancel()
    {
        if (!IsOpen || !escapable) return;
        Close();
    }

    protected void SetButtonsInteractable(bool value)
    {
        foreach (var btn in menuButtons)
        {
            if (btn != null)
                btn.gameObject.SetActive(value);
        }
    }
}