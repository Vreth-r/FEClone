using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryBase : MonoBehaviour, IInventory
{
    [SerializeField] protected int capacity = -1; // -1 is unlimited

    public List<ItemInstance> items = new();

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

    public virtual bool Move(
        IInventory to,
        ItemInstance item)
    {
        if (!to.CanAdd(item))
        {
            return false;
        }

        if (!this.Remove(item))
        {
            return false;
        }

        to.Add(item);
        return true;
    }
}