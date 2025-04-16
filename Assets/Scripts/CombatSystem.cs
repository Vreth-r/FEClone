using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PLACEHOLDER
public class CombatSystem
{
    public static void Attack(Unit attacker, Unit defender)
    {
        int damage = Mathf.Max(0, attacker.attack - defender.defense);
        defender.currentHP -= damage;
        Debug.Log($"{attacker.name} attacked {defender.name} for {damage} damage");
    }
}
