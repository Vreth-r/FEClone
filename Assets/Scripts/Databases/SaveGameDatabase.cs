using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Databases/SaveGame Databse")]
public class SaveGameDatabase : AddressableDatabase<SaveGameData>
{
    // load all items with the Item label
    protected override string Label => "SaveGame";

    public IReadOnlyList<SaveGameData> GetByHasData(bool hasData)
    {
        return items.Where(i => i.hasData == hasData).ToList();
    }

    public SaveGameData GetBySlotName(SaveGameData.SlotName slotName)
    {
        foreach (SaveGameData item in items)
        {
            if (item.slotName == slotName)
                return item;
        }
        return null;
    }

    public IReadOnlyList<SaveGameData> GetAll()
    {
        return items;
    }
}