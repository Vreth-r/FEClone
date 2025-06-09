using UnityEngine;
using System.Collections.Generic;

// Ultra persistent script for "global" variable tracking and scene management
/*
save from anywhere:
SaveSystem.SaveGame(0);

load from anywhere:
if (SaveSystem.SaveExists(0))
{
    SaveSystem.LoadGame(0);
}
*/
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // currency
    public int Gold;

    // Convoy Inventory (upgrading to its own class later)
    public List<Item> convoy = new();

    // Optional: Global flags
    public HashSet<string> globalFlags = new();

    public List<string> recruitedUnitIDs;

    public Transform[] playerSpawnPositions; // in editor

    [Header("Databases")]
    public TerrainDatabase terrainDatabase;
    public UnitDatabase unitDatabase;
    public ItemDatabase itemDatabase;
    public SkillDatabase skillDatabase;
    public UnitClassDatabase unitClassDatabase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        terrainDatabase.Init();
        unitDatabase.Init();
        //skillDatabase.OnEnable(); handle this stupid fucking shit later
        //itemDatabase.OnEnable();
        //unitClassDatabase.OnEnable();
    }

    // Gold Management
    public void AddGold(int amount) => Gold += amount;
    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            return true;
        }
        return false;
    }

    // Convoy Management
    public void AddToConvoy(Item item) => convoy.Add(item);
    public void RemoveFromConvoy(Item item) => convoy.Remove(item);
    public bool ConvoyContains(Item item) => convoy.Contains(item);

    // Unit Management
    public void RecruitUnit(string unitID)
    {
        if (!recruitedUnitIDs.Contains(unitID)) recruitedUnitIDs.Add(unitID);
    }

    public bool IsUnitRecruited(string unitID) => recruitedUnitIDs.Contains(unitID);
}