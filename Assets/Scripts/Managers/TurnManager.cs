using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState { Player, Enemy}
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public TurnState currentTurn = TurnState.Player;

    private void Awake() => Instance = this;

    // for when all roster units are tapped and the game auto ends the turn
    public void TryEndPlayerTurn()
    {
        foreach(Unit u in UnitManager.Instance.playerUnits)
        {
            if(u.state != UnitState.Tapped)
            {
                return;
            }
        }
        currentTurn = TurnState.Enemy;
        Debug.Log($"Turn changed to: {currentTurn}");
    }
}
