using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ShopMenuTheSecond : UIMenuBase
{
    [Header("Shop Stock")]
    private List<Item> shopStock;

    private Label goldLabel;
    private ScrollView buyView;
    private ScrollView sellView;
    private ScrollView upgradeView;

    protected override void OnCreate()
    {
        goldLabel = Root.Q<Label>("GoldLabel");
        buyView = Root.Q<ScrollView>("BuyContent");
        sellView = Root.Q<ScrollView>("SellContent");
        upgradeView = Root.Q<ScrollView>("UpgradeContent");

        if (shopStock != null)
        {
            shopStock.Clear();
        }
        else
        {
            shopStock = new List<Item>();
        }
        shopStock.AddRange(GameManager.Instance.itemDatabase.GetAll());
        RefreshAll();
    }

    public override void OnOpen()
    {
        RefreshAll();
    }

    // =========================
    // REFRESH
    // =========================

    private void RefreshAll()
    {
        RefreshGold();
        PopulateBuy();
        PopulateSell();
        PopulateUpgrades();
    }

    private void RefreshGold()
    {
        goldLabel.text = $"Gold: {GameManager.Instance.Gold}";
    }

    // =========================
    // BUY
    // =========================

    private void PopulateBuy()
    {
        buyView.Clear();

        foreach (var item in shopStock)
        {
            buyView.Add(CreateItemRow(
                item,
                $"Buy ({GetBuyPrice(item)}g)",
                () => BuyItem(item)
            ));
        }
    }

    private void BuyItem(Item item)
    {
        int price = GetBuyPrice(item);

        if (GameManager.Instance.SpendGold(price))
        {
            PlayerInventory.Instance.Add(new ItemInstance(item));
            Debug.Log($"Bought: {item.itemName}");
            RefreshGold();
            PopulateSell();
            PopulateUpgrades();
        }
    }

    // =========================
    // SELL
    // =========================

    private void PopulateSell()
    {
        sellView.Clear();

        foreach (var instance in PlayerInventory.Instance.Items)
        {
            sellView.Add(CreateItemRow(
                instance.Definition,
                $"Sell ({GetSellPrice(instance)}g)",
                () => SellItem(instance)
            ));
        }
    }

    private void SellItem(ItemInstance instance)
    {
        PlayerInventory.Instance.Remove(instance);
        GameManager.Instance.AddGold(GetSellPrice(instance));

        RefreshAll();
    }

    // =========================
    // UPGRADE
    // =========================

    private void PopulateUpgrades()
    {
        upgradeView.Clear();

        foreach (var instance in PlayerInventory.Instance.Items)
        {
            if (!instance.IsWeapon)
                continue;

            upgradeView.Add(CreateItemRow(
                instance.Definition,
                $"Upgrade ({GetUpgradeCost(instance)}g)",
                () => UpgradeWeapon(instance)
            ));
        }
    }

    private void UpgradeWeapon(ItemInstance instance)
    {
        int cost = GetUpgradeCost(instance);

        if (GameManager.Instance.SpendGold(GetUpgradeCost(instance)))
        {
            // upgrade logic goes here
            Debug.Log("Upgraded whatever (placeholder)");

            RefreshAll();
        }
    }

    // =========================
    // UI HELPERS
    // =========================

    private VisualElement CreateItemRow(
        Item item,
        string buttonText,
        System.Action action)
    {
        var row = new VisualElement();
        row.AddToClassList("item-row");

        // var icon = new Image { sprite = item.icon };
        // icon.AddToClassList("item-icon");

        var info = new VisualElement();
        info.AddToClassList("item-info");

        var name = new Label(item.itemName);
        name.AddToClassList("item-name");

        var desc = new Label(item.description);
        desc.AddToClassList("item-desc");

        info.Add(name);
        info.Add(desc);

        var button = new Button(action) { text = buttonText };
        button.AddToClassList("item-action");

        //row.Add(icon);
        row.Add(info);
        row.Add(button);

        return row;
    }

    // =========================
    // PRICING
    // =========================

    // all placeholders
    private int GetBuyPrice(Item item)
    {
        return item.itemType switch
        {
            ItemType.Weapon => 100,
            ItemType.Consumable => 25,
            _ => 10
        };
    }

    private int GetSellPrice(ItemInstance instance)
    {
        return GetBuyPrice(instance.Definition) / 2;
    }

    private int GetUpgradeCost(ItemInstance instance)
    {
        return 75;
    }
}
