using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tactics RPG/Consumable")]
public class ConsumableItem : Item
{
    public int healAmount = 10;
    
    private void OnEnable()
    {
        itemType = ItemType.Consumable;
    }

    public override void Use(Unit user, Unit target)
    {
        Debug.Log("Used an item");
        // hm thinking of mechanics here
        /*
        was thinking having effect fields for each instance and enabling them on creation 
        so the item can do its thing, but i may think of something better soon
        */
    }
}
