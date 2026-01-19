using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Yarn.Unity;

// Ultra persistent script for "global" variable tracking and scene/gameflow management
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

    [Header("Game Variables")]
    // currency
    public int Gold;

    // Convoy Inventory (upgrading to its own class later)
    public List<Item> convoy = new();

    // Optional: Global flags
    public HashSet<string> globalFlags = new();

    public List<string> recruitedUnitIDs;

    public Transform[] playerSpawnPositions; // in editor

    [Header("Databases")]
    //public TerrainDatabase terrainDatabase;
    // public UnitDatabase unitDatabase;
    public ItemDatabase itemDatabase;
    // public SkillDatabase skillDatabase;
    // public UnitClassDatabase unitClassDatabase;

    [Header("Yarn")]
    public DialogueRunner MasterYarnRunner;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(MasterYarnRunner);

        //terrainDatabase.Init(); // this one is dead for now until i have a use for it
        //unitDatabase.Init();
        //skillDatabase.OnEnable(); handle this stupid fucking shit later
        //itemDatabase.Initialize();
        //itemDatabase.DebugPrintThatShit();
        //unitClassDatabase.OnEnable();

        Gold = 999;
    }

    // game stuff
    public IEnumerator InitializeAllDatabases()
    {
        bool done = false;
        InitializeDatabases(() => done = true);

        yield return new WaitUntil(() => done);
    }

    public void InitializeDatabases(System.Action onComplete)
    {
        int pending = 0;

        void Track(System.Action initCall)
        {
            pending++;
            initCall();
        }

        void OnOneDone()
        {
            pending--;
            if (pending <= 0)
            {
                onComplete?.Invoke();
            }
        }

        Track(() => itemDatabase.Initialize(OnOneDone));
        // soon (tm)
        // Track(() => skillDatabase.Initialize(OnOneDone));
        // Track(() => classDatabase.Initialize(OnOneDone));
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

// code utilities
public static class CoroutineExtensions
{
    public static IEnumerator ContinueWith(this IEnumerator routine, System.Action onComplete)
    {
        yield return routine;
        onComplete?.Invoke();
    }
}