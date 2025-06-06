using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelMapData
{
    public string mapID;
    public string displayName;

    public List<string> terrainKey;
    public string tileCSV;
    public List<TileData> tiles; // fallback option, not be be used really
    public List<UnitSpawnData> playerUnits;
    public List<UnitSpawnData> enemyUnits;
    // add more later
}