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

    // WIP
    private void SpawnPlayerUnits()
    {
        // connects to existing save system for player data
        if (SaveSystem.SaveExists(0))
        {
            SaveSystem.LoadGame(0);
        }
        else
        {
        //  create a default party if no save data exists
        //  CreateDefaultParty();
            Debug.LogError($"player save doesnt exist");
        }
    }

    // idk exactly how to do this but i imagine it would look something like this


    // private void CreateDefaultParty()
    // {
    //     
    //     idk how many characters default party is
    //
    //     string[] defaultUnitIDs = { "Ylru" };
    //    
    //     for (int i = 0; i < defaultUnitIDs.Length && i < playerSpawnPositions.Length; i++)
    //     {
    //         SpawnDefaultUnit(defaultUnitIDs[i], playerSpawnPositions[i]);
    //     }
    // }

    // private void SpawnDefaultUnit(string unitID, Transform spawnPoint)
    // {
    //     UnitData unitData = GameManager.Instance.unitDatabase.GetByID(unitID);
    //     if (unitData == null)
    //     {
    //         Debug.LogError($"unit not found in database: {unitID}");
    //     }

    //     Vector3Int gridPos = new Vector3Int(
    //         Mathf.RoundToInt(spawnPoint.position.x),
    //         Mathf.RoundToInt(spawnPoint.position.y),
    //         0 
    //     );
        
    //     Unit playerUnit = UnitSpawner.Instance.SpawnUnitFromTemplate(unitData, gridPos);
    //     playerUnit.team = Team.Player;
    //     UnitManager.Instance.RegisterUnit(playerUnit);
    // }
}