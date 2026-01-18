using UnityEngine;

public class PlayerInventory : InventoryBase
{
    public static PlayerInventory Instance { get; private set; }

    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // convenience for PlayerInventory calls
    public ItemInstance CreateAndAdd(Item item)
    {
        var instance = new ItemInstance(item);
        Add(instance);
        return instance;
    }
}