using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TurnState { Player, Enemy}
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public int turnNum = -1;

    public TurnState currentTurn = TurnState.Enemy;

    public event Action<int> OnTurnFlip;

    private void Awake() => Instance = this;

    // broadcast turn flip and the number
    public void TurnFlip()
    {
        currentTurn = currentTurn == TurnState.Player ? TurnState.Enemy : TurnState.Player;
        turnNum++;
        OnTurnFlip?.Invoke(turnNum);
        Debug.Log($"{turnNum} | {currentTurn}");
    }

    // for when all roster units are tapped and the game auto ends the turn
    public void TryEndPlayerTurn()
    {
        foreach (Unit u in UnitManager.Instance.playerUnits)
        {
            if (u.state != UnitState.Tapped)
            {
                return;
            }
        }
        //TurnFlip();
        Debug.Log($"Turn changed to: {currentTurn}");
    }
}
