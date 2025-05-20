using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string sceneName;
    public Vector2 playerPosition;
    public string timestamp;

    // Expand this later
    public List<string> recruitedUnits;
    public int gold;
}