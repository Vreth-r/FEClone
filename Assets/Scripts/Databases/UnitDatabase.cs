using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tactics RPG/Unit Database")]
public class UnitDatabase : Database<UnitData>
{
    public static UnitDatabase Instance; // singleton reference

    public void Init()
    {
        base.Initialize(); // Init db
        if (Instance == null) Instance = this; // Assign singleton
    }
}