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
        DontDestroyOnLoad(partyContainer);
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
            Transform player = partyContainer.transform.GetChild(i);
            player.SetParent(null); 

            // add to UnitManager
            Unit unit = player.GetComponent<Unit>();
            if (unit != null)
            {
                unit.transform.position = GridManager.Instance.CellToWorld(spawnPositions[i]);
                unit.GridPosition = (Vector2Int)spawnPositions[i];

                UnitManager.Instance.RegisterUnit(unit);
            }
        }
    }

    public bool HasActiveParty()
    {
        return partyContainer.transform.childCount > 0;
    }
}