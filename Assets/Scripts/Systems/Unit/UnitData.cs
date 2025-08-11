using UnityEngine;
using System.Collections.Generic;

//  This one is for defining the characters initial stats and shit

[CreateAssetMenu(menuName = "Tactics RPG/Unit Data")]
public class UnitData : ScriptableObject, IIdentifiable
{
    public string unitID; // their name
    public string ID => unitID;

    public string unitName;
    public string unitTitle;
    [TextArea] public string unitDescription;

    public Team team;
    public UnitClass startingClass;
    public int level = 1;

    public int maxHP = 20;
    public int strength;
    public int arcane;
    public int defense;
    public int speed;
    public int skill;
    public int resistance;
    public int luck;

    public List<Item> startingInventory;
    public Item equippedItem;
    public Sprite combatSprite;
    public GameObject animationPrefab;
}