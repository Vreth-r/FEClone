using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
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
            _ = RunEnemyTurnAsync();
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
    
    public async UniTask RunEnemyTurnAsync()
    {
        Debug.Log("=== Enemy Turn Start ===");
        await UniTask.Delay(500);

        foreach (var enemy in UnitManager.Instance.enemyUnits)
        {
            if (enemy == null) continue;
            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai == null) continue;

            await ai.RunTurnAsync();
            await UniTask.Delay(250);
        }

        Debug.Log("=== Enemy Turn End ===");
        TurnFlip();
    }
}
