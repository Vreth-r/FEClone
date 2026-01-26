using UnityEngine;
using Steamworks;
using System.Collections.Generic;

// these are just examples/for testing
public enum Achievement : int
{
    ACH_FIRST_GOLD,
    ACH_FIRST_GOLD_SPENT,
    ACH_1M_GOLD_EARNED_LT_250_CURRENT, // example for multi variable
    ACH_FIRST_ENEMY,
    ACH_FINISH_LIBRARY
}
// stat type enum
public enum GameStat : int
{
    TOTAL_GOLD_EARNED,
    TOTAL_GOLD_SPENT,
    MAX_GOLD_BALANCE,
    TOTAL_ENEMIES_DEFEATED,
}

public class StatsAndAchievementManager : MonoBehaviour
{

    // these are just some generic examples 
    // i gotta rename the classes this is getting confusing
    private Dictionary<Achievement, Achievement_t> achievements = new Dictionary<Achievement, Achievement_t>
    {
        {Achievement.ACH_FIRST_GOLD, new Achievement_t(Achievement.ACH_FIRST_GOLD, "Pocket Change", "Earn your first gold")},
        {Achievement.ACH_FIRST_GOLD_SPENT, new Achievement_t(Achievement.ACH_FIRST_GOLD_SPENT, "Trip to the Shops", "Spent your first gold")},
        {Achievement.ACH_1M_GOLD_EARNED_LT_250_CURRENT, new Achievement_t(Achievement.ACH_1M_GOLD_EARNED_LT_250_CURRENT, "Financial Mismanagement", "Had at least 1 000 000 gold, later had less than 250")},
        {Achievement.ACH_FIRST_ENEMY, new Achievement_t(Achievement.ACH_FIRST_ENEMY, "A Tarnished Soul", "Killed your first enemy")},
        {Achievement.ACH_FINISH_LIBRARY, new Achievement_t(Achievement.ACH_FINISH_LIBRARY, "Schools out!", "Finish the Library Scene")}
    };


    // thought it would be better to store stats in a dictionary for easier lookup, although now the stat stat type is stored twice, might revisit later
    // also these are just examples
    public Dictionary<GameStat, GameStatistic> statistics = new Dictionary<GameStat, GameStatistic>
    {
        {GameStat.TOTAL_GOLD_EARNED, new GameStatistic(GameStat.TOTAL_GOLD_EARNED, "Total Gold Earned", StatDataType.INT)},
        {GameStat.TOTAL_GOLD_SPENT, new GameStatistic(GameStat.TOTAL_GOLD_SPENT, "Total Gold Spent", StatDataType.INT)},
        {GameStat.MAX_GOLD_BALANCE, new GameStatistic(GameStat.MAX_GOLD_BALANCE, "Highest Gold Balance", StatDataType.INT)},
        {GameStat.TOTAL_ENEMIES_DEFEATED, new GameStatistic(GameStat.TOTAL_ENEMIES_DEFEATED, "Total Enemies Defeated", StatDataType.INT)}        
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
    public bool UpdateStat(GameStat statID, float? floatData=null, int? intData=null, float? sessionCountData=null)
    {
        statistics.TryGetValue(statID,  out GameStatistic statistic);
        switch (statistic.dataType)
        {
            case StatDataType.FLOAT:
                if (floatData != null)
                {
                    Debug.Log($"{statID} updated to {floatData}");
                    statistic.SetStatistic((float)floatData); // cast because of float? (this isn't a question lol)
                    CheckAchievements();
                    if (SteamManager.Initialized)
                        statistic.SendUserStat();
                    return true;
                }
                break;
            case StatDataType.INT:
                if (intData != null)
                {
                    Debug.Log($"{statID} updated to {intData}");
                    statistic.SetStatistic((int)intData); // cast because of int?
                    CheckAchievements();
                    if (SteamManager.Initialized)
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
                    if (SteamManager.Initialized)
                        statistic.SendUserStat();
                    return true;
                }
                break;
        }
        Debug.Log($"{statistic.dataType} data null, {statID} not updated");
        return false;
    }

    // realized it was good/useful to have this 
     public void AddToStatistic(GameStat statID, float? floatData=null, int? intData=null, float? sessionCountData=null)
    {
        statistics.TryGetValue(statID, out GameStatistic statistic);
        switch (statistic.dataType)
        {
            case StatDataType.FLOAT:
                if (floatData != null)
                {
                    statistic.AddToStatistic((float)floatData);
                    UpdateStat(statID, floatData: statistic.GetStat());
                    return;
                }
                break;
            case StatDataType.INT:
                if (intData != null)
                {
                    statistic.AddToStatistic((int)intData);
                    UpdateStat(statID, intData: (int)statistic.GetStat());
                    return;
                }
                break;
        }
        Debug.Log($"{statistic.dataType} data null, {statID} not updated");
        return;
    }

    // this is also kind of clunky, might change later, only to check stat based achievements
    // this might actually be needed because not all achievements are stat based...
    // or a there could be a flag in the Achievements_t class that tells the program if it is stat based or not
    private void CheckAchievements ()
    {
        // just loop through and see if they have been done
        // it would look nicer if each achievement was a child of the Achievements_t class with a
        // success checker function but it would probably be annoying to pass in the user stats 
        foreach (Achievement_t achievement in achievements.Values)
        {
            if (achievement.achieved)
                continue;

            switch (achievement.achievementID)
            {
                case Achievement.ACH_FIRST_GOLD:
                    if (statistics[GameStat.TOTAL_GOLD_EARNED].GetStat() >= 1)
                    {
                        UnlockAchievement(achievement);
                    }
                    break;
                case Achievement.ACH_FIRST_GOLD_SPENT:
                    if (statistics[GameStat.TOTAL_GOLD_SPENT].GetStat() >= 1)
                        {
                            UnlockAchievement(achievement);
                        }
                    break;
                case Achievement.ACH_1M_GOLD_EARNED_LT_250_CURRENT:
                    if (statistics[GameStat.MAX_GOLD_BALANCE].GetStat() >= 1000000 && GameManager.Instance.Gold <= 250)
                    {
                        UnlockAchievement(achievement);
                    }
                    break;
                case Achievement.ACH_FIRST_ENEMY:
                    if (statistics[GameStat.TOTAL_ENEMIES_DEFEATED].GetStat() >= 1)
                    {
                        UnlockAchievement(achievement);
                    }
                    break;
            }
        }
    }

    private void UnlockAchievement(Achievement_t achievement) {
        Debug.Log($"Achievement {achievement.achievementName} unlocked");
		achievement.achieved = true;
        if (SteamManager.Initialized)
    		SteamUserStats.SetAchievement(achievement.achievementID.ToString());
	}
    public void UnlockAchievement(Achievement achievementID) { // overloaded to be able to use enum, for non stat based achievements
        Achievement_t achievement = achievements[achievementID];
        Debug.Log($"Achievement {achievement.achievementName} unlocked");
		achievement.achieved = true;
        if (SteamManager.Initialized)
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
                foreach (Achievement_t achievement in achievements.Values)
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
                foreach (GameStatistic statistic in statistics.Values)
                {
                    switch (statistic.dataType)
                    {
                        case StatDataType.FLOAT:
                            SteamUserStats.GetStat(statistic.statID.ToString(), out float floatData);
                            statistic.SetStatistic(floatData);
                            break;
                        case StatDataType.INT:
                            SteamUserStats.GetStat(statistic.statID.ToString(), out int intData);
                            statistic.SetStatistic(intData);
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
    public GameStat statID;
    public string statName;
    public StatDataType dataType;
    public float floatData;
    public int intData;
    public float sessionCountData;
    public float sessionLength;

    public GameStatistic (GameStat statID, string statName, StatDataType dataType)
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
                Debug.Log($"Sent {statID} = {floatData} to steam");
                SteamUserStats.SetStat(statID.ToString(), floatData);
                break;
            case StatDataType.INT:
                Debug.Log($"Sent {statID} = {intData} to steam");
                SteamUserStats.SetStat(statID.ToString(), intData);
                break;
            case StatDataType.AVGRATE:
                Debug.Log($"Sent {statID} = {sessionCountData / sessionLength} to steam");
                SteamUserStats.UpdateAvgRateStat(statID.ToString(), sessionCountData, sessionLength);
                break;
        }
    }
    
}