using UnityEngine;

public class UnitInventory : InventoryBase
{
    [SerializeField] private Unit owner;

    protected void Reset()
    {
        capacity = 5; // default
    }

    public bool TransferTo(UnitInventory target, ItemInstance item)
    {
        if (!target.CanAdd(item))
        {
            return false;
        }

        Remove(item);
        target.Add(item);
        return true;
    }
}