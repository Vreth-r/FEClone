using System;
using System.Collections.Generic;
using UnityEngine;

// This one is for JSON and is for saving data throughout a playthrough

[Serializable]
public class SavedUnitData
{
    public string unitID;
    public string unitClassName;
    public int level;
    public int currentHP;
    public int maxHP;
    public int strength;
    public int arcane;
    public int defense;
    public int speed;
    public int skill;
    public int resistance;
    public int luck;

    public List<string> inventoryIDs;
    public string equippedItemID;
    public Vector2Int gridPosition;
}