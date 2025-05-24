using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tactics RPG/Unit Roster")]
public class UnitRoster : ScriptableObject
{
    public List<UnitData> startingUnits;
}