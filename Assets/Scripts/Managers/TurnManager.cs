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
        Debug.Log($"Turn changed to: {currentTurn} | Turn Number: {turnNum}");

        // untap all units
        if (currentTurn == TurnState.Player)
        {
            foreach (Unit u in UnitManager.Instance.playerUnits)
            {
                u.state = UnitState.Idle;
            }
        }
        // run enemy turns
        if(currentTurn == TurnState.Enemy)
        {
            StartCoroutine(RunEnemyTurn());
        }
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
        TurnFlip();
    }

    public void TryEndEnemyTurn()
    {
        //whateverlogic
        TurnFlip();
    }
    
    public IEnumerator RunEnemyTurn()
{
    Debug.Log("=== Enemy Turn Start ===");

    // Small pre-delay for clarity / pacing
    yield return new WaitForSeconds(0.5f);

    foreach (var enemy in UnitManager.Instance.enemyUnits)
    {
        if (enemy == null) continue;

        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai == null) continue;

        Debug.Log($"[TurnManager] Starting turn for enemy: {enemy.unitName} | ID: {enemy.unitID}");

        // Begin AI's turn
        //ai.SetTurnInProgress(true);
        ai.RunTurn();

        // Wait until it reports finished
        yield return new WaitUntil(() => ai.IsDone());

        Debug.Log($"[TurnManager] {enemy.unitName} finished turn.");

        // Add small delay between enemies for pacing
        yield return new WaitForSeconds(0.25f);
    }

    Debug.Log("=== Enemy Turn End ===");

    // Flip back to player after all enemies are done
    TurnFlip();
}


}
