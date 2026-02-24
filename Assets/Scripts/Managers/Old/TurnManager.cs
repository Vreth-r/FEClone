using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;

public enum TurnState { Player, Enemy}
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public int turnNum = -1;

    public TurnState currentTurn = TurnState.Enemy;

    public event Action<int> OnTurnFlip; // different event for non effect/action based objects
    public string levelCompleteYarnNode = "";

    private void Awake() => Instance = this;

    // broadcast turn flip and the number
    public void TurnFlip()
    {
        currentTurn = currentTurn == TurnState.Player ? TurnState.Enemy : TurnState.Player;
        turnNum++;
        OnTurnFlip?.Invoke(turnNum);
        Debug.Log($"Turn changed to: {currentTurn} | Turn Number: {turnNum}");

        // check if the game is done, which is really just if all enemy units are dead and none are spawning (for right now)
        if (UnitManager.Instance.enemyUnits.Count == 0 && UnitSpawner.Instance.spawnEvents.Count == 0 && levelCompleteYarnNode != "")
        {
            ControlsManager.Instance.SetContext(InputContext.Cutscene);
            GameManager.Instance.MasterYarnRunner.StartDialogue(levelCompleteYarnNode);
        }
        
        // untap all units
        if (currentTurn == TurnState.Player)
        {
            // move this to when the enemy turn starts, then disable the cursor movement during enemy turn
            // go to lowest ID that is still alive
            foreach (var key in UnitManager.Instance.playerUnits.Keys.OrderBy(k => k))
            {
                // Guard against missing keys (dictionary changed during iteration)
                if (!UnitManager.Instance.playerUnits.TryGetValue(key, out var value)) continue;
                CursorController.Instance.SetCurrentGridPosition((Vector3Int)value.GridPosition);
                CursorController.Instance.UpdateCursorTile();
                break;
            }
            foreach (var entry in UnitManager.Instance.playerUnits)
            {
                entry.Value.state = UnitState.Idle;
                entry.Value.statBonuses.TickDownModifiers();
            }
        }
        // run enemy turns
        if(currentTurn == TurnState.Enemy)
        {
            foreach (var entry in UnitManager.Instance.enemyUnits)
            {
                entry.Value.state = UnitState.Idle;
                entry.Value.statBonuses.TickDownModifiers();
            }
            _ = RunEnemyTurnAsync();
        }
    }

    // for when all roster units are tapped and the game auto ends the turn
    public void TryEndPlayerTurn()
    {
        foreach (var entry in UnitManager.Instance.playerUnits)
        {
            if (entry.Value.state != UnitState.Tapped)
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
        var enemiesSnapshot = new Dictionary<string, Unit>(UnitManager.Instance.enemyUnits);
        foreach (var entry in enemiesSnapshot)
        {
            if (entry.Value == null || entry.Value.state != UnitState.Idle) continue;
            if (entry.Value.IsDead) continue;
            EnemyAI ai = entry.Value.GetComponent<EnemyAI>();
            if (ai == null) continue;

            await ai.RunTurnAsync();
            await UniTask.Delay(250);
        }

        Debug.Log("=== Enemy Turn End ===");
        TurnFlip();
    }
}
