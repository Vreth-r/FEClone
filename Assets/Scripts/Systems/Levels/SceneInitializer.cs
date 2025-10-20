using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

public class SceneInitializer : MonoBehaviour
{
    [Header("Scene References")]
    // unit spawns set in editor for each level manager
    public Vector3Int[] playerUnitSpawnPositions; // grid position
    
    private List<Unit> preplacedEnemies = new List<Unit>();

    private void Start()
    {
        // clear previous state
        UnitManager.Instance.ClearAllUnits();
        
        FindAllEnemiesInScene();

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
                unit.GridPosition = (Vector2Int)GridManager.Instance.WorldToCell(unit.gameObject.transform.position);
                preplacedEnemies.Add(unit);
                UnitManager.Instance.RegisterUnit(unit);
            }
        }
    }

    private void SpawnPlayerUnits()
    {
        if (PlayerPersistor.Instance.HasStoredParty())
        {

            PlayerPersistor.Instance.RestorePartyToScene(playerUnitSpawnPositions);
        }
        else
        {
            Debug.LogError("no party found");
        }
    }
}