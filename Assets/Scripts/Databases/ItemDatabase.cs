using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tactics RPG/Item Database")]
public class ItemDatabase : Database<Item>
{
    public static ItemDatabase Instance; // singleton reference

    public void Init()
    {
        base.Initialize(); // Init db
        if (Instance == null) Instance = this; // Assign singleton
    }
}
