using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

public class SceneInitializer : MonoBehaviour
{
    [Header("Scene References")]
    // unit spawns set in editor for each level manager
    public Vector3[] playerUnitSpawnPositions; // world position
    
    private List<Unit> preplacedEnemies = new List<Unit>();

    private void Start()
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
        Vector3Int[] spawnGridPositions = new Vector3Int[playerUnitSpawnPositions.Length];
        for (int i = 0; i < playerUnitSpawnPositions.Length; i++)
        {
            spawnGridPositions[i] = GridManager.Instance.WorldToCell(playerUnitSpawnPositions[i]);
        }

        if (PlayerPersistor.Instance.HasStoredParty())
        {

            PlayerPersistor.Instance.RestorePartyToScene(spawnGridPositions);
        }
        else
        {
            Debug.LogError("no party found");
        }
    }
}