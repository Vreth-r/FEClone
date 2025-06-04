using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelMapData
{
    public string mapID;
    public string displayName;
    public int width;
    public int height;

    public List<TileData> tiles;
    public List<UnitSpawnData> playerUnits;
    public List<UnitSpawnData> enemyUnits;
    // add more later
}