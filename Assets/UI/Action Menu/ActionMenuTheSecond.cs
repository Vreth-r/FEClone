using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ActionMenuTheSecond 
    : UIMenuBase, IMenuWithData<ActionMenuData>
{
    // hello thomas.
    private VisualElement buttonContainer;
    private VisualElement selectionIndicator;

    private readonly List<Button> buttons = new();
    
    private readonly List<System.Action> buttonActions = new();
    private int selectedIndex;

    private float inputCooldown = 0.2f;
    private float lastInputTime;

    private Unit unit;

    protected override void OnCreate()
    {
        buttonContainer = Root.Q<VisualElement>("button-container");
        selectionIndicator = Root.Q<VisualElement>("selection-indicator");

        ControlsManager.Instance.OnSubmit += HandleSubmit;
        ControlsManager.Instance.OnCancel += HandleCancel;
    }

    public override void OnClose()
    {
        ControlsManager.Instance.OnSubmit -= HandleSubmit;
        ControlsManager.Instance.OnCancel -= HandleCancel;
    }

    public void SetData(ActionMenuData data)
    {
        unit = data.unit;
        BuildButtons(data.actions);

        PositionMenu(data.worldPosition);
        Select(0);
    }

    private void BuildButtons(List<UnitAction> actions)
    {
        buttonContainer.Clear();
        buttons.Clear();

        foreach (var action in actions)
        {
            if (!action.IsAvailable(unit))
                continue;

            var btn = new Button();
            btn.text = action.actionName;
            btn.AddToClassList("action-button");

            // btn.clicked += async () =>
            // {
            //     manager.CloseTopMenu();
            //     await action.TryExecuteAsync(unit);
            // };

            buttonActions.Add(async () =>
            {
                manager.CloseTopMenu();
                await action.TryExecuteAsync(unit);
            });

            int index = buttonActions.Count - 1;
            btn.clicked += () => buttonActions[index].Invoke();

            btn.style.color = action.color;
            buttonContainer.Add(btn);
            buttons.Add(btn);
        }
    }

    private void PositionMenu(Vector3 worldPos)
    {
        if (Camera.main == null)
        return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (screenPos.z < 0)
            return;

        // Convert to panel space (THIS is the missing step)
        Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(
            Root.panel,
            screenPos
        );

        Root.style.position = Position.Absolute;
        Root.style.left = panelPos.x + 20f;
        Root.style.top = panelPos.y - 10f;
    }

    public override void OnOpen()
    {
        ControlsManager.Instance.SetContext(InputContext.Menu);
    }

    private void Select(int index)
    {
        selectedIndex = index;

        for (int i = 0; i < buttons.Count; i++)
            buttons[i].SetEnabled(i == index);

        var target = buttons[index];
        selectionIndicator.style.visibility = Visibility.Visible;

        selectionIndicator.style.top = target.layout.y + 10;
    }

    // basically Update
    public override void Tick(float deltaTime)
    {
        if (buttons.Count == 0) return;

        Vector2 nav = ControlsManager.Instance.NavigateInput;
        if (Time.time - lastInputTime < inputCooldown || nav == Vector2.zero)
            return;

        if (Mathf.Abs(nav.y) > Mathf.Abs(nav.x)) // if youre navigating up or down
        {
            int dir = (int)Mathf.Sign(-nav.y);
            selectedIndex = (selectedIndex + dir + buttons.Count) % buttons.Count;
            Select(selectedIndex);
            lastInputTime = Time.time;
        }
    }

    private void HandleSubmit()
    {
        if (ControlsManager.Instance.CurrentContext != InputContext.Menu)
            return;

        buttonActions[selectedIndex]?.Invoke();
    }

    private void HandleCancel()
    {
        manager.CloseTopMenu();
    }
}

public struct ActionMenuData
{
    public Unit unit;
    public Vector3 worldPosition;
    public List<UnitAction> actions;
}

