using System;
using System.Collections.Generic;
using UnityEngine;

// Will be expanded later

[Serializable]
public class SaveData
{
    public string sceneName;
    public List<SavedUnitData> savedUnits;
    public int gold;
    public List<string> convoyItemIDs;
    public string timestamp;

    public Vector2 playerPosition;
}