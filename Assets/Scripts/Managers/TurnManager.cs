using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState { Player, Enemy}
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public TurnState currentTurn = TurnState.Player;

    private void Awake() => Instance = this;

    public void EndTurn()
    {
        currentTurn = currentTurn == TurnState.Player ? TurnState.Enemy : TurnState.Player;
        Debug.Log($"Turn changed to: {currentTurn}");
    }
}
