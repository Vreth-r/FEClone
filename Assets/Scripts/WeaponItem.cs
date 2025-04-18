using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tactics RPG/Weapon")]
public class WeaponItem : Item // holy coding btw
{
    public WeaponType weaponType;
    public int might; // The extra strength or mag a unit gets for using this item
    public int hit; // the extra hit a unit gets for using this item
    public int avoid; // the extra avoidance a unit gets for using this item
    public int crit; // the extra crit a unit gets for using this item
    // might add more of these for primary stats cause i like the concept
    
    public int durability = 50; // debating on whether to have this or do what fates did

    private void OnEnable()
    {
        itemType = ItemType.Weapon;
    }

    public override void Use(Unit user, Unit target)
    {
        Debug.Log($"{user.name} attacks {target.name} with {itemName}!");
        // placeholder for now, combat logic maybe later 
    }
}
