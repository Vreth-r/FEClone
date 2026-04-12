using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class InventoryManagerMenu : UIMenuBase
{
    private ScrollView playerInventoryView;
    private ScrollView unitRosterView;
    private ScrollView unitInventoryView;
    private Label goldLabel;

    private UnitRosterEntry selectedUnit;

    protected override void OnCreate()
    {
        // Set up references to the UI elements
        playerInventoryView = Root.Q<ScrollView>("playerInventory");
        unitRosterView = Root.Q<ScrollView>("unitRosterList");
        unitInventoryView = Root.Q<ScrollView>("unitInventory");
        goldLabel = Root.Q<Label>("playerInventoryLabel");

        // Populate player inventory and unit roster
        PopulatePlayerInventory();
        PopulateUnitRoster();
    }

    private void PopulatePlayerInventory()
    {
        playerInventoryView.Clear();

        foreach (var item in PlayerInventory.Instance.Items)
        {
            var itemRow = CreateItemRow(item, () => SelectItemForUnit(item));
            playerInventoryView.Add(itemRow);
        }
    }

    private void PopulateUnitRoster()
    {
        unitRosterView.Clear();

        foreach (var entry in GameManager.Instance.PlayerRoster.Entries)
        {
            var unitRow = CreateUnitRow(entry, () => SelectUnit(entry));

            unitRosterView.Add(unitRow);
        }
    }

    private VisualElement CreateItemRow(ItemInstance item, System.Action onSelect)
    {
        var row = new VisualElement();
        row.AddToClassList("item-row");

        var icon = new Image { sprite = item.Definition.icon };
        icon.AddToClassList("item-icon");

        var name = new Label(item.Definition.itemName);
        var desc = new Label(item.Definition.description);
        name.AddToClassList("item-name");
        desc.AddToClassList("item-desc");

        row.Add(icon);
        row.Add(name);
        row.Add(desc);

        var button = new Button(onSelect) { text = "Select" };
        row.Add(button);

        return row;
    }

    private VisualElement CreateUnitRow(UnitRosterEntry unit, System.Action onSelect)
    {
        var row = new VisualElement();
        row.AddToClassList("item-row");

        var name = new Label(unit.UnitPrefab.unitName);
        row.Add(name);

        var button = new Button(onSelect) { text = "Select" };
        row.Add(button);

        return row;
    }

    private void SelectUnit(UnitRosterEntry unit)
    {
        selectedUnit = unit;
        PopulateUnitInventory();
    }

    private void PopulateUnitInventory()
    {
        if (selectedUnit == null)
            return;

        unitInventoryView.Clear();

        foreach (var item in selectedUnit.RuntimeState.Inventory.Items)
        {
            var itemRow = CreateItemRow(item, () => DeselectItemFromUnit(item));
            unitInventoryView.Add(itemRow);
        }
    }

    private void SelectItemForUnit(ItemInstance item)
    {
        if (selectedUnit != null && PlayerInventory.Instance.Items.Contains(item))
        {
            PlayerInventory.Instance.Move(selectedUnit.RuntimeState.Inventory, item);
            PopulatePlayerInventory();
            PopulateUnitInventory();
        }
    }

    private void DeselectItemFromUnit(ItemInstance item)
    {
        if (selectedUnit != null && selectedUnit.RuntimeState.Inventory.Items.Contains(item))
        {
            selectedUnit.RuntimeState.Inventory.Move(PlayerInventory.Instance, item);
            PopulateUnitInventory();
            PopulatePlayerInventory();
        }
    }

    public override void OnOpen()
    {
        PopulatePlayerInventory();
        PopulateUnitRoster();
    }
}
