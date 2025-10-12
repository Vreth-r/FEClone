using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
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
        Debug.Log($"Initializing level: {gameObject.scene.name}");
        
        // clear previous state
        UnitManager.Instance.ClearAllUnits();
        
        // find and register all pre-placed enemies in the scene
        FindAllEnemiesInScene();
        RegisterPreplacedUnits();
        
        // spawn player units from save data
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
        // get player party from save data
        List<SavedUnitData> playerParty = GetPlayerPartyFromSave();
        
        if (playerParty == null || playerParty.Count == 0)
        {
            Debug.LogError("no player save data");
            return;
        }

        // spawn each player unit at the assigned spawn points
        for (int i = 0; i < playerParty.Count && i < playerSpawnPositions.Length; i++)
        {
            SpawnPlayerUnit(playerParty[i], playerSpawnPositions[i]);
        }
    }

    private List<SavedUnitData> GetPlayerPartyFromSave()
    {
        // connects to existing save system
        if (SaveSystem.SaveExists(0))
        {
            // this would return List<SavedUnitData> from the save file
            return null; // placeholder
        }
        return null;
    }

    private void SpawnPlayerUnit(SavedUnitData savedData, Transform spawnPoint)
    {
        Unit playerUnit = UnitSpawner.Instance.SpawnUnitFromSaveData(
            savedData, 
            Vector3Int.RoundToInt(spawnPoint.position)
        );
        
        playerUnit.team = Team.Player;
        UnitManager.Instance.RegisterUnit(playerUnit);
    }
}