using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionMenu : NavMenu, IGameMenu
{
    [Header("UI References")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject background;
    [SerializeField] private Vector2 size = new Vector2(125, 30);

    private Unit _unit;
    private readonly List<GameObject> _spawnedButtons = new();

    public override MenuType MenuID => MenuType.ActionMenu;

    public override void Open()
    {
        // override just to ensure indicator refreshes properly
        base.Open();
    }

    public void Open(Unit unit, Vector3 worldPos, List<UnitAction> availableActions)
    {
        _unit = unit;
        ClearButtons();
        menuButtons.Clear();

        // position menu near unit in screen space
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos + new Vector3(1f, 1.5f, 0));
        background.transform.position = screenPos;

        // create a button for each available action
        foreach (var action in availableActions)
        {
            if (!action.IsAvailable(unit))
                continue;

            var buttonGO = Instantiate(buttonPrefab, buttonParent);
            var bTransform = buttonGO.GetComponent<RectTransform>();
            bTransform.sizeDelta = size;
            _spawnedButtons.Add(buttonGO);

            var button = buttonGO.GetComponent<Button>();
            var label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();

            label.text = action.actionName;
            label.color = action.color;

            // action on click
            button.onClick.AddListener(async () =>
            {
                UIManager.Instance.CloseTopMenu();
                await action.TryExecuteAsync(unit);
            });

            menuButtons.Add(button);// add to nav list
        }

        base.Open();
    }

    private void ClearButtons()
    {
        foreach (var b in _spawnedButtons)
        {
            if (b != null)
                Destroy(b);
        }
        _spawnedButtons.Clear();
        menuButtons.Clear();
    }

    public override void Close()
    {
        base.Close();
        ClearButtons();
    }
}
