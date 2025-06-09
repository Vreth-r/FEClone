using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// abstract class scriptable for items, decently simple system

public enum ItemType
{
    Weapon,
    Consumable
}
public abstract class Item : ScriptableObject, IIdentifiable
{
    public string itemName;
    [SerializeField] public string itemID;
    public string ID => itemID;
    [TextArea] public string description;
    public ItemType itemType;
    public Sprite icon;

    // For attack ranges
    public int minRange = 1;
    public int maxRange = 1;

    public List<EffectInstance> effects;

    public abstract void Use(Unit user, Unit target); // to be overridden in other files
}
