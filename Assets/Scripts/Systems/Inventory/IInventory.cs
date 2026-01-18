using System.Collections.Generic;

public interface IInventory
{
    IReadOnlyList<ItemInstance> Items { get; }

    bool CanAdd(ItemInstance item);
    bool Add(ItemInstance item);
    bool Remove(ItemInstance item);
}