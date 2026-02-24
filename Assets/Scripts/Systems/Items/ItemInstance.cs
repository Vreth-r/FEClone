using System;
using UnityEngine;

[Serializable]
public class ItemInstance
{
    [SerializeField] private string itemID;
    public string ID => itemID;
    [System.NonSerialized] private Item definition;

    // runtime
    [SerializeField] private int currentDurability;
    [SerializeField] private int maxDurability;

    public Item Definition
    {
        get
        {
            if (definition == null)
            {
                definition = GameManager.Instance.itemDatabase.GetByID(itemID);
            }

            return definition;
        }
    }

    // construction

    public ItemInstance(Item item)
    {
        definition = item;
        itemID = item.ID;
        switch (item)
        {
            case WeaponItem weapon:
                maxDurability = weapon.durability;
                currentDurability = maxDurability;
                break;
            
            case ConsumableItem:
                maxDurability = 1;
                currentDurability = 1;
                break;
            
            default:
                maxDurability = 1;
                currentDurability = 1;
                break;
        }
    }

    // type helpers

    public bool IsWeapon => definition is WeaponItem;
    public bool IsConsumable => definition is ConsumableItem;

    public WeaponItem AsWeapon => definition as WeaponItem;
    public ConsumableItem AsConsumable => definition as ConsumableItem;

    // durability

    public int Durability => currentDurability;
    public int MaxDurability => maxDurability;
    public bool UsesDurability => IsWeapon;

    public bool IsBroken => UsesDurability && currentDurability <= 0;

    public void ReduceDurability(int amount = 1)
    {
        if (!UsesDurability)
        {
            return;
        }

        currentDurability = Mathf.Max(0, currentDurability - amount);
    }

    public void Repair(int amount)
    {
        if (!UsesDurability)
        {
           return; 
        }
        
        currentDurability = maxDurability;
    }

    // usage
    public void Use(Unit user, Unit target)
    {
        if (IsBroken)
        {
            Debug.Log($"{definition.itemName} is broken!");
            return;
        }

        definition.Use(user, target);

        if (UsesDurability)
        {
            ReduceDurability();
        }
        else if (IsConsumable)
        {
            currentDurability = 0;
        }
    }

    // utility

    public bool ShouldBeRemovedFromInventory()
    {
        return IsConsumable && currentDurability <= 0;
    }
}