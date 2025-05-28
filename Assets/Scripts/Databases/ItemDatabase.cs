using UnityEngine;
using System.Collections.Generic;

// Item Database, keeps a reference for each item scriptable object

[CreateAssetMenu(menuName = "Tactics RPG/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public static ItemDatabase Instance; // singleton reference

    public List<Item> allItems; // List for editor population

    private Dictionary<string, Item> itemLookup; // Dicitionary for O(1) reference

    private void OnEnable()
    {
        Instance = this; // assign singleton
        itemLookup = new Dictionary<string, Item>(); // init dict

        foreach (var item in allItems)
        {
            if (item != null && !itemLookup.ContainsKey(item.itemID)) // if item exists and not already in the database
            {
                itemLookup[item.itemID] = item; // add to DB
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
