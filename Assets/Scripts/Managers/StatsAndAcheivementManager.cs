using UnityEngine;
using Steamworks;
using UnityEngine.SocialPlatforms.Impl;

public class StatsAndAchievementManager : MonoBehaviour
{
    private enum Achievement : int
    {
        ACH_1M_GOLD_EARNED,
        ACH_100K_GOLD_SPENT,
        ACH_1M_GOLD_EARNED_LT_250_CURRENT,
        ACH_1K_ENEMIES,
    }

    // these are just some generic examples 
    private Achievement_t[] Achievements = new Achievement_t[] { // apparently _t postfix means typedef
		new Achievement_t(Achievement.ACH_1M_GOLD_EARNED, "Financial Freedom", "Earn 1 000 000 gold"),
		new Achievement_t(Achievement.ACH_100K_GOLD_SPENT, "Big Spender", "Spent 100 000 gold"),
		new Achievement_t(Achievement.ACH_1M_GOLD_EARNED_LT_250_CURRENT, "Financial Mismanagement", "Had at least 1 000 000 gold, later had less than 250"),
		new Achievement_t(Achievement.ACH_1K_ENEMIES, "Killer", "Killed 1 000 enemies")
	};

    private CGameID gameID; // proj. iron game id (steam)

    // if stats were received from steam
    private bool requestedStats;
    private bool statsValid;

    // if stats should be stored currently
    private bool shouldStoreStats;

    // stats of current session
    private float tickGameStart;
    private float gameDuration;

    // overall stats
    private int totalGoldEarned; // these are just examples of stuff...
    private int totalGoldSpent;
    private int maxGoldBalance;
    private int totalEnemiesDefeated;

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

        requestedStats = false;
        statsValid = false;
    }

    private void Update()
    {
        if (!SteamManager.Initialized)
            return;

        // it seems there have been some changes to the library since they last updated the example so ill revisit this later
        /*
        if (!requestedStats)
        {
             // if the steam is not loaded still (tho idk how it would even get here) then there is no point in trying to get stats
            if (!SteamManager.Initialized) 
            {
                requestedStats = true;
                return;
            }
            bool retrieveSuccess = SteamUserStats.RequestUserStats();

        }
        */

        // not sure this is the best implementation, it may be better to call from the game manager to register the achievements rather than checking each frame
        foreach (Achievement_t achievement in Achievements)
        {
            if (achievement.achieved)
                continue;

            switch (achievement.achievementID)
            {
                case Achievement.ACH_1M_GOLD_EARNED:
                    if (totalGoldEarned >= 1000000)
                    {
                        UnlockAchievement(achievement);
                    }
                    break;
                case Achievement.ACH_100K_GOLD_SPENT:
                    if (totalGoldSpent >= 100000)
                        {
                            UnlockAchievement(achievement);
                        }
                    break;
                case Achievement.ACH_1M_GOLD_EARNED_LT_250_CURRENT:
                    if (maxGoldBalance >= 1000000 && GameManager.Instance.Gold <= 250)
                    {
                        UnlockAchievement(achievement);
                    }
                    break;
                case Achievement.ACH_1K_ENEMIES:
                    if (totalEnemiesDefeated >= 1000)
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

		// Store stats end of frame
		shouldStoreStats = true;
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
                foreach (Achievement_t achievement in Achievements)
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

                // this looks like it could be shortened greatly with a generalized stats class
                SteamUserStats.GetStat("totalGoldEarned", out totalGoldEarned);
                SteamUserStats.GetStat("totalGoldSpent", out totalGoldSpent);
                SteamUserStats.GetStat("maxGoldBalance", out maxGoldBalance);
                SteamUserStats.GetStat("totalEnemiesDefeated", out totalEnemiesDefeated);

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
