using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryBase : MonoBehaviour, IInventory
{
    [SerializeField] protected int capacity = -1; // -1 is unlimited

    protected List<ItemInstance> items = new();

    public IReadOnlyList<ItemInstance> Items => items;

    public virtual bool CanAdd(ItemInstance item)
    {
        if (capacity < 0)
        {
            return true;
        }

        return items.Count < capacity;
    }

    public virtual bool Add(ItemInstance item)
    {
        if (!CanAdd(item))
        {
            return false;
        }

        items.Add(item);
        return true;
    }

    public virtual bool Remove(ItemInstance item)
    {
        return items.Remove(item);
    }

    public static bool Move(
        IInventory from,
        IInventory to,
        ItemInstance item)
    {
        if (!to.CanAdd(item))
        {
            return false;
        }

        if (!from.Remove(item))
        {
            return false;
        }

        to.Add(item);
        return true;
    }
}