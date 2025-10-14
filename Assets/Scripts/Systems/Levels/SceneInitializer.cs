using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

public class SceneInitializer : MonoBehaviour
{
    [Header("Scene References")]
    public Transform[] playerSpawnPositions;
    
    private List<Unit> preplacedEnemies = new List<Unit>();

    private void Start()
    {
        InitializeLevel();
    }

    public void InitializeLevel()
    {   
        // clear previous state
        UnitManager.Instance.ClearAllUnits();
        
        FindAllEnemiesInScene();
        RegisterPreplacedUnits();
        
        SpawnPlayerUnits();
    }

    private void FindAllEnemiesInScene()
    {
        preplacedEnemies.Clear();
        
        Unit[] allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        foreach (Unit unit in allUnits)
        {
            if (unit.team == Team.Enemy)
            {
                preplacedEnemies.Add(unit);
            }
        }
    }

    private void RegisterPreplacedUnits()
    {
        foreach (Unit enemy in preplacedEnemies)
        {
            if (enemy != null)
            {
                UnitManager.Instance.RegisterUnit(enemy);
            }
        }
    }

    private void SpawnPlayerUnits()
    {
        Vector3Int[] spawnGridPositions = new Vector3Int[playerSpawnPositions.Length];
        for (int i = 0; i < playerSpawnPositions.Length; i++)
        {
            spawnGridPositions[i] = new Vector3Int(
                Mathf.RoundToInt(playerSpawnPositions[i].position.x),
                Mathf.RoundToInt(playerSpawnPositions[i].position.y),
                0
            );
        }

        if (PlayerPersistor.Instance.HasActiveParty())
        {

            PlayerPersistor.Instance.RestorePartyToScene(spawnGridPositions);
        }
        else
        {
            Debug.LogError("no party found");
        }
    }
}