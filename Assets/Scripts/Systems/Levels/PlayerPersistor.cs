using TMPro;
using UnityEngine;

// creates an empty game object to store the player units while switching levels
public class PlayerPersistor : MonoBehaviour
{
    public static PlayerPersistor Instance;

    private GameObject partyContainer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        partyContainer = new GameObject("PartyContainer");
        partyContainer.transform.SetParent(transform);
        //DontDestroyOnLoad(partyContainer);
    }

    public void StorePartyInContainer()
    {
        Unit[] allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        foreach (Unit unit in allUnits)
        {
            if (unit.team == Team.Player)
            {
                unit.transform.SetParent(partyContainer.transform);
                UnitManager.Instance.UnregisterUnit(unit);
            }
        }
    }

    public void RestorePartyToScene(Vector3Int[] spawnPositions)
    {
        // move players back to the scene and position them
        for (int i = 0; i < partyContainer.transform.childCount && i < spawnPositions.Length; i++)
        {
            Transform playerUnit = partyContainer.transform.GetChild(i);
            playerUnit.SetParent(null);

            // add to UnitManager
            Unit unit = playerUnit.GetComponent<Unit>();
            if (unit != null)
            {
                unit.transform.position = GridManager.Instance.CellToWorld(spawnPositions[i]) + new Vector3(0.5f, 0.5f, 0);;
                unit.GridPosition = (Vector2Int)spawnPositions[i];
                playerUnit.SetParent(GameObject.Find("Units").GetComponent<Transform>());
                UnitManager.Instance.RegisterUnit(unit);
            }
        }
    }

    public bool HasStoredParty()
    {
        // check PartyContainer for party members
        if (partyContainer.transform.childCount > 0)
        {
            return true;
        }

        // when party isnt stored yet (aka first level), check preplaced units
        Unit[] allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        bool foundPlayerUnits = false;
        
        foreach (Unit unit in allUnits)
        {
            if (unit.team == Team.Player)
            {
                foundPlayerUnits = true;
                break; // found at least one, stop searching
            }
        }
        
        // if we found player units in the scene, store them all
        if (foundPlayerUnits)
        {
            StorePartyInContainer();
        }
        
        return foundPlayerUnits;
    }
}