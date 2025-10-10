using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Terrain Tile Data", menuName = "Tactics RPG/Terrain Tile Data")]
public class TerrainTile : Tile, IIdentifiable
{
    public string terrainName; // Name for later
    public string ID => terrainName;

    [Header("Movement")]
    public int moveCost = 1; // 1 is normal, 2 for forests, etc\
    public bool impassable = false; // impassable terrain is impassable by movement, can be teleported through

    [Header("Tag Blocking")]
    // changing this to be list based instead later
    public bool blocksArmored = false;
    public bool blocksMounted = false;
    public bool blocksNonFlying = false;
    public bool ignoreForFlying = true;

    [Header("Combat Bonuses")]
    // flat bonuses for certain stats
    public int hitBonus = 0;
    public int avoidBonus = 0;
    public int defenseBonus = 0;
}