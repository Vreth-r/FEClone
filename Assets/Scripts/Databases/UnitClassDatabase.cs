using UnityEngine;
using System.Collections.Generic;

// UnitClass Database, keeps a reference for each skill scriptable object

[CreateAssetMenu(menuName = "Tactics RPG/Class Database")]
public class UnitClassDatabase : Database<UnitClass>
{
    public static UnitClassDatabase Instance; // singleton reference

    public void Init()
    {
        base.Initialize(); // Init db
        if (Instance == null) Instance = this; // assign singleton
    }
}