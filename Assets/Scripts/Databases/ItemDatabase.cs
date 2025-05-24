using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tactics RPG/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public static ItemDatabase Instance;

    public List<Item> allItems;

    private Dictionary<string, Item> itemLookup;

    private void OnEnable()
    {
        Instance = this;
        itemLookup = new Dictionary<string, Item>();

        foreach (var item in allItems)
        {
            if (item != null && !itemLookup.ContainsKey(item.itemID))
            {
                itemLookup[item.itemID] = item;
            }
        }
    }

    public static Item GetItemByID(string id)
    {
        if (Instance == null || Instance.itemLookup == null)
        {
            Debug.LogError("ItemDatabase not initialized.");
            return null;
        }

        if (Instance.itemLookup.TryGetValue(id, out Item item))
            return item;

        Debug.LogWarning($"Item ID {id} not found.");
        return null;
    }
}
