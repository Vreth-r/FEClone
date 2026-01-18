using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Databases/Item Databse")]
public class ItemDatabase : AddressableDatabase<Item>
{
    // load all items with the Item label
    protected override string Label => "Item";

    public IReadOnlyList<WeaponItem> Weapons =>
        items.OfType<WeaponItem>().ToList();

    public IReadOnlyList<ConsumableItem> Consumables => 
        items.OfType<ConsumableItem>().ToList();
    
    public IReadOnlyList<Item> GetByType(ItemType type)
    {
        return items.Where(i => i.itemType == type).ToList();
    }

    public IReadOnlyList<Item> GetAll()
    {
        return items;
    }

    public void DebugPrintThatShit()
    {
        Debug.Log("We out here");
        foreach (var item in items)
        {
            Debug.Log($"{item.ID}");
        }
    }
}