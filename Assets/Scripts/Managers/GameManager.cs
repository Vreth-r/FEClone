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

    // Optional: Global flags
    public HashSet<string> globalFlags = new();

    public Transform[] playerSpawnPositions; // in editor

    public UnitRoster PlayerRoster { get; private set;}

    [Header("Databases")]
    public UnitAddressableDatabase unitDatabase;
    public ItemDatabase itemDatabase;
    // public SkillDatabase skillDatabase;
    // public UnitClassDatabase unitClassDatabase;

    [Header("Yarn")]
    public DialogueRunner MasterYarnRunner;

    private bool databasesInitialized = false;

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

        //unitDatabase.Init();
        //skillDatabase.OnEnable(); handle this stupid fucking shit later
        //unitClassDatabase.OnEnable();

        PlayerRoster = new UnitRoster();
        Gold = 999; 
    }

    // game stuff
    public IEnumerator InitializeAllDatabases(float timeout = 10f)
    {
        if (databasesInitialized)
        {
            Debug.Log("[GameManager] Databases already initialized.");
            yield break;
        }

        Debug.Log("[GameManager] Initializing databases...");

        int total = 0;
        int completed = 0;

        void RegisterDB(System.Action<System.Action> initCall)
        {
            total++;
            initCall(() =>
            {
                completed++;
                Debug.Log($"[GameManager] DB Initialized {completed}/{total}");
            });
        }

        RegisterDB(cb => itemDatabase.Initialize(cb));
        RegisterDB(cb => unitDatabase.Initialize(cb));

        float timer = 0f;

        while (completed < total)
        {
            timer += Time.deltaTime;

            if (timer > timeout)
            {
                Debug.LogError("[GameManager] DATABASE INIT TIMEOUT — forcing continue");
                break;
            }

            yield return null;
        }

        databasesInitialized = true;

        Debug.Log($"[GameManager] Database init complete ({completed}/{total})");
    }

    // anything for a new game gets made here, assume everything is empty on run and load save data only when told
    public void OnNewGame()
    {
        GameObject ylruGO = Instance.unitDatabase.GetPrefab("ylru");
        if(ylruGO != null)
        {
            Instance.PlayerRoster.Add(ylruGO.GetComponent<Unit>());
            UnitRosterEntry ylru = Instance.PlayerRoster.Get("ylru");
        }
        else
        {
            Debug.Log("No Ylru in database");
        }
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
        Track(() => unitDatabase.Initialize(OnOneDone));
        // soon (tm)
        // Track(() => skillDatabase.Initialize(OnOneDone));
        // Track(() => classDatabase.Initialize(OnOneDone));
    }

    // for when a level ends
    // public static void SyncSceneUnitsToRoster()
    // {
    //     foreach (Unit unit in Object.FindObjectsByType<Unit>(FindObjectsSortMode.None))
    //     {
    //         if (unit.team != Team.Player)
    //             continue;

    //         var roster = GameManager.Instance.PlayerRoster;
    //         var entry = roster.Get(unit.unitID);

    //         if (entry != null)
    //         {
    //             entry.RuntimeState = unit.ExtractRuntimeState();
    //         }
    //     }
    // }

    // Gold Management
    public void AddGold(int amount)
    {
        Gold += amount;
        StatsAndAchievementManager.Instance.AddToStatistic(GameStat.TOTAL_GOLD_EARNED, intData: amount); // this is just an example
        if (Gold > StatsAndAchievementManager.Instance.statistics[GameStat.MAX_GOLD_BALANCE].GetStat()) // maybe this is a bit long
            StatsAndAchievementManager.Instance.UpdateStat(GameStat.MAX_GOLD_BALANCE, intData: amount);
    
    }
    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            //StatsAndAchievementManager.Instance.AddToStatistic(GameStat.TOTAL_GOLD_SPENT, intData: amount); // this is just an example
            return true;
        }
        return false;
    }


    public void loadPlayerRoster (UnitRoster unitRoster)
    {
        PlayerRoster = unitRoster;
    }
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