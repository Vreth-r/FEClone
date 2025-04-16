using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team { Player, Enemy }
public class Unit : MonoBehaviour
{
    public Team team;
    public int movementRange = 3;
    public int attackRange = 2; // will prob be changed later when weapons are implemented as this will be variable depending on weapon type
    public int currentHP = 10;
    public int attack = 3;
    public int defense = 1;

    public Vector2Int GridPosition { get; set; }

    private void Start()
    {
        GridPosition = (Vector2Int)GridManager.Instance.WorldToCell(transform.position);
        UnitManager.Instance.RegisterUnit(this); // Tell the unit manager this thing exists
    }

}
