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
    public IEnumerator InitializeAllDatabases()
    {
        bool done = false;
        InitializeDatabases(() => done = true);

        yield return new WaitUntil(() => done);
    }

    // anything for a new game gets made here, assume everything is empty on run and load save data only when told
    public void OnNewGame()
    {
        Instance.PlayerRoster.Add(Instance.unitDatabase.GetPrefab("ylru").GetComponent<Unit>());
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
        StatsAndAchievementManager.Instance.AddToStatistic(StatsAndAchievementManager.Stat.TOTAL_GOLD_EARNED, intData: amount); // this is just an example
        if (Gold > StatsAndAchievementManager.Instance.statistics[StatsAndAchievementManager.Stat.MAX_GOLD_BALANCE].GetStat()) // maybe this is a bit long
            StatsAndAchievementManager.Instance.UpdateStat(StatsAndAchievementManager.Stat.MAX_GOLD_BALANCE, intData: amount);
    
    }
    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            StatsAndAchievementManager.Instance.AddToStatistic(StatsAndAchievementManager.Stat.TOTAL_GOLD_SPENT, intData: -amount); // this is just an example
            return true;
        }
        return false;
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