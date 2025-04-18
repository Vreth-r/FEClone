using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Stores data for each terrain type a tile could take

[CreateAssetMenu(menuName = "Tactics RPG/Terain Tile")]
public class TerrainTile : ScriptableObject
{
    [Header("Tile Visual")] 
    public TileBase tileVisual; // the tile (not the tile art, the tile from unity)
    public string terrainName; // Name for later

    [Header("Movement")]
    public int moveCost = 1; // 1 is normal, 2 for forests, etc\
    public bool impassable = false; // impassable terrain is impassable by movement, can be teleported through

    [Header("Tag Blocking")]
    // changing this to be list based instead later
    public bool blocksArmored = false;
    public bool blocksMounted = false;
    public bool ignoreForFlying = true;

    [Header("Combat Bonuses")]
    // flat bonuses for certain stats
    public int avoidBonus = 0;
    public int defenseBonus = 0;
}
