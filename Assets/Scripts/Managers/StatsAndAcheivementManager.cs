using UnityEngine;
using Steamworks;
using System.Collections.Generic;

public class StatsAndAchievementManager : MonoBehaviour
{
    private enum Achievement : int
    {
        ACH_1M_GOLD_EARNED,
        ACH_100K_GOLD_SPENT,
        ACH_1M_GOLD_EARNED_LT_250_CURRENT,
        ACH_1K_ENEMIES,
    }
// stat type enum
public enum Stat : int
{
    TOTAL_GOLD_EARNED,
    TOTAL_GOLD_SPENT,
    MAX_GOLD_BALANCE,
    TOTAL_ENEMIES_DEFEATED,
}


    // these are just some generic examples 
    private Achievement_t[] achievementsList = new Achievement_t[] { // apparently _t postfix means typedef
		new Achievement_t(Achievement.ACH_1M_GOLD_EARNED, "Financial Freedom", "Earn 1 000 000 gold"),
		new Achievement_t(Achievement.ACH_100K_GOLD_SPENT, "Big Spender", "Spent 100 000 gold"),
		new Achievement_t(Achievement.ACH_1M_GOLD_EARNED_LT_250_CURRENT, "Financial Mismanagement", "Had at least 1 000 000 gold, later had less than 250"),
		new Achievement_t(Achievement.ACH_1K_ENEMIES, "Killer", "Killed 1 000 enemies")
	};


    // thought it would be better to store stats in a dictionary for easier lookup, although now the stat stat type is stored twice, might revisit later
    // also these are just examples
    public Dictionary<Stat, GameStatistic> statistics = new Dictionary<Stat, GameStatistic>
    {
        {Stat.TOTAL_GOLD_EARNED, new GameStatistic(Stat.TOTAL_GOLD_EARNED, "Total Gold Earned", StatDataType.INT)},
        {Stat.TOTAL_GOLD_SPENT, new GameStatistic(Stat.TOTAL_GOLD_SPENT, "Total Gold Spent", StatDataType.INT)},
        {Stat.MAX_GOLD_BALANCE, new GameStatistic(Stat.MAX_GOLD_BALANCE, "Highest Gold Balance", StatDataType.INT)},
        {Stat.TOTAL_GOLD_EARNED, new GameStatistic(Stat.TOTAL_GOLD_EARNED, "Total Enemies Defeated", StatDataType.INT)}        
    };

    private CGameID gameID; // proj. iron game id (steam)

    // if stats were received from steam
    private bool requestedStats; // prob dont need
    private bool statsValid; // prob do need, just dont know where to put it yet

    // time of current session
    private float tickGameStart;
    private float sessionLength;

    protected Callback<UserStatsReceived_t> userStatsReceived;
    protected Callback<UserStatsStored_t> userStatsStored;
    protected Callback<UserAchievementStored_t> userAchievementStored;

    public static StatsAndAchievementManager Instance;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        DontDestroyOnLoad(gameObject); // idk how exactly you are handling the scene loading because i didn't look at it, but if this isn't needed then remove it
    }

    void OnEnable()
    {
        if (!SteamManager.Initialized)
            return; // cant do anything if steam manager not initialized

        gameID = new CGameID(SteamUtils.GetAppID());

        userStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
        userStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
		userAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);

        // since there aren't really like "per game" things (its just one big game), time is just stored from the beginning
        // im also not entirely sure we need to be storing time based stats other than for interest sake
        tickGameStart = Time.time;

        requestedStats = false;
        statsValid = false;
    }

    // generalized stat updater for calling from other classes mainly
    public bool UpdateStat(Stat statID, float? floatData=null, int? intData=null, float? sessionCountData=null)
    {
        statistics.TryGetValue(statID,  out GameStatistic statistic);
        switch (statistic.dataType)
        {
            case StatDataType.FLOAT:
                if (floatData != null)
                {
                    statistic.SetStatistic((float)floatData); // cast because of float? (this isn't a question lol)
                    CheckAchievements();
                    statistic.SendUserStat();
                    return true;
                }
                break;
            case StatDataType.INT:
                if (intData != null)
                {
                    statistic.SetStatistic((int)intData); // cast because of int?
                    CheckAchievements();
                    statistic.SendUserStat();
                    return true;
                }
                break;
            case StatDataType.AVGRATE:
                if (sessionCountData != null)
                {
                    sessionLength = Time.time - tickGameStart;
                    statistic.SetStatistic((float)sessionCountData, sessionLength);
                    CheckAchievements();
                    statistic.SendUserStat();
                    return true;
                }
                break;
        }
        Debug.Log($"{statistic.dataType} data null, {statID} not updated");
        return false;
    }

    // realized it was good/useful to have this 
     public void AddToStatistic(Stat statID, float? floatData=null, int? intData=null, float? sessionCountData=null)
    {
        statistics.TryGetValue(statID, out GameStatistic statistic);
        switch (statistic.dataType)
        {
            case StatDataType.FLOAT:
                if (floatData != null)
                {
                    float currentStatValue = statistic.GetStat();
                    UpdateStat(statID, floatData: statistic.floatData + currentStatValue);
                    return;
                }
                break;
            case StatDataType.INT:
                if (intData != null)
                {
                    int currentStatValue = (int)statistic.GetStat();
                    UpdateStat(statID, intData: statistic.intData + currentStatValue);
                    return;
                }
                break;
        }
        Debug.Log($"{statistic.dataType} data null, {statID} not updated");
        return;
    }

    private void CheckAchievements () // this is also kind of clunky, might change later
    {
        // just loop through and see if they have been done
        // it would look nicer if each achievement was a child of the Achievements_t class with a
        // success checker function but it would probably be annoying to pass in the user stats 
        foreach (Achievement_t achievement in achievementsList)
        {
            if (achievement.achieved)
                continue;

            switch (achievement.achievementID)
            {
                case Achievement.ACH_1M_GOLD_EARNED:
                    if (statistics[Stat.TOTAL_GOLD_EARNED].GetStat() >= 1000000)
                    {
                        UnlockAchievement(achievement);
                    }
                    break;
                case Achievement.ACH_100K_GOLD_SPENT:
                    if (statistics[Stat.TOTAL_GOLD_SPENT].GetStat() >= 100000)
                        {
                            UnlockAchievement(achievement);
                        }
                    break;
                case Achievement.ACH_1M_GOLD_EARNED_LT_250_CURRENT:
                    if (statistics[Stat.MAX_GOLD_BALANCE].GetStat() >= 1000000 && GameManager.Instance.Gold <= 250)
                    {
                        UnlockAchievement(achievement);
                    }
                    break;
                case Achievement.ACH_1K_ENEMIES:
                    if (statistics[Stat.TOTAL_ENEMIES_DEFEATED].GetStat() >= 1000)
                    {
                        UnlockAchievement(achievement);
                    }
                    break;
            }
        }
    }

    private void UnlockAchievement(Achievement_t achievement) {
		achievement.achieved = true;

		SteamUserStats.SetAchievement(achievement.achievementID.ToString());
	}

    void OnUserStatsReceived(UserStatsReceived_t callback)
    {
        if (!SteamManager.Initialized) //idk we keep doing this bruh
            return;

        // "// we may get callbacks for other games' stats arriving, ignore them"
        if ((ulong)gameID == callback.m_nGameID)
        {
            if (EResult.k_EResultOK == callback.m_eResult) {
				Debug.Log("Received stats and achievements from Steam");

				statsValid = true;

                // load achievements 
                foreach (Achievement_t achievement in achievementsList)
                {
                    bool ret = SteamUserStats.GetAchievement(achievement.achievementID.ToString(), out achievement.achieved);
                    if (ret)
                    {
                        achievement.achievementName = SteamUserStats.GetAchievementDisplayAttribute(achievement.achievementID.ToString(), "name");
                        achievement.achievementDescription = SteamUserStats.GetAchievementDisplayAttribute(achievement.achievementID.ToString(), "desc");
                    }
                    else
                    {
                        Debug.Log($"Failed to get achievement. {achievement.achievementID} may not be registered");
                    }
                }

                // load stats into dictionary from stored stuff
                foreach (var statistic in statistics)
                {
                    switch (statistic.Value.dataType)
                    {
                        case StatDataType.FLOAT:
                            SteamUserStats.GetStat(statistic.Key.ToString(), out float floatData);
                            statistic.Value.SetStatistic(floatData);
                            break;
                        case StatDataType.INT:
                            SteamUserStats.GetStat(statistic.Key.ToString(), out int intData);
                            statistic.Value.SetStatistic(intData);
                            break;
                    }
                }
            }
            else {
				Debug.Log($"RequestStats - failed, {callback.m_eResult}");
			}
        }
    }
    void OnUserStatsStored(UserStatsStored_t callback)
    {
        // "// we may get callbacks for other games' stats arriving, ignore them"
		if ((ulong)gameID == callback.m_nGameID) {
			if (EResult.k_EResultOK == callback.m_eResult) {
				Debug.Log("StoreStats - success");
			}

			else if (EResult.k_EResultInvalidParam == callback.m_eResult) {
				// "// One or more stats we set broke a constraint. They've been reverted,"
				// "// and we should re-iterate the values now to keep in sync."
				Debug.Log("StoreStats - some failed to validate");
				// "// Fake up a callback here so that we re-load the values."
				UserStatsReceived_t fakeCallback = new UserStatsReceived_t();
				fakeCallback.m_eResult = EResult.k_EResultOK;
				fakeCallback.m_nGameID = (ulong)gameID;
				OnUserStatsReceived(fakeCallback);
			}
			else {
				Debug.Log($"StoreStats - failed, {callback.m_eResult}");
			}
		}
    }
    void OnAchievementStored(UserAchievementStored_t callback)
    {
        if ((ulong)gameID == callback.m_nGameID)
        {
            if (callback.m_nMaxProgress == 0)
            {
                Debug.Log($"Achievement {callback.m_rgchAchievementName} unlocked");
            }
            else {
				Debug.Log($"Achievement '{callback.m_rgchAchievementName}' progress callback, ({callback.m_nCurProgress}, {callback.m_nMaxProgress})");
			}
        }
    }

    // Achievement class
    private class Achievement_t
    {
        public Achievement achievementID;
        public string achievementName;
        public string achievementDescription;
        public bool achieved;

        public Achievement_t (Achievement achievementID, string achievementName, string achievementDescription)
        {
            this.achievementID = achievementID;
            this.achievementName = achievementName;
            this.achievementDescription = achievementDescription;
            this.achieved = false;
        }
    }
}

// these are the data types supported by steamworks
public enum StatDataType
{
    INT,
    FLOAT,
    AVGRATE // dont actually think we'll end up using this... its for stuff like points/min which i dont think is super relevant

}


// game stat class to not have a very large amount of variables to keep track of (kinda)
public class GameStatistic
{
    public StatsAndAchievementManager.Stat statID;
    public string statName;
    public StatDataType dataType;
    public float floatData;
    public int intData;
    public float sessionCountData;
    public float sessionLength;

    public GameStatistic (StatsAndAchievementManager.Stat statID, string statName, StatDataType dataType)
    {
        this.statID = statID;
        this.statName = statName;
        this.dataType = dataType;
    }

    public float GetStat () // float bc int can just be cast, might change this later to be better.
    {
        switch (dataType)
        {
            case StatDataType.FLOAT:
                return floatData;
            case StatDataType.INT:
                return intData;
            case StatDataType.AVGRATE:
                return 0f;
        }
        return 0f;
    }

    // setters, these dont send the user stat because it would make a loop of callbacks with the current implementation
    public void SetStatistic (float floatData)
    {
        this.floatData = floatData;
    }
    public void SetStatistic (int intData)
    {
        this.intData = intData;
    }
    public void SetStatistic (float sessionCountData, float sessionLength) 
    {
        this.sessionCountData = sessionCountData;
        this.sessionLength = sessionLength;
    }

    // i think i forgot to use these lol, ill use them later
    public void AddToStatistic (float floatData)
    {
        this.floatData += floatData;
    }
    public void AddToStatistic (int intData)
    {
        this.intData += intData;
    }


    public void SendUserStat () // send stat to steam
    {
        switch (dataType)
        {
            case StatDataType.FLOAT:
                SteamUserStats.SetStat(statID.ToString(), floatData);
                break;
            case StatDataType.INT:
                SteamUserStats.SetStat(statID.ToString(), intData);
                break;
            case StatDataType.AVGRATE:
                SteamUserStats.UpdateAvgRateStat(statID.ToString(), sessionCountData, sessionLength);
                break;
        }
    }
    
}