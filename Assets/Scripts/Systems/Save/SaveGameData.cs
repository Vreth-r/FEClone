
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SaveGame/SaveGameData")]
public class SaveGameData : ScriptableObject
{
    public SlotName slotName; // idk if needed
    public bool hasData = false;

    // tracked variables 
    private int gameLevel;
    private int gold;
    private UnitRoster playerUnitRoster;

    // settings
    [Range (0f, 1f)] private float masterVolume;
    [Range (0f, 1f)] private float uiVolume;
    [Range (0f, 1f)] private float musicVolume;
    [Range (0f, 1f)] private float gameplayVolume;

    private float zoomLevel;
    private float gameSpeed;

    public bool SaveGame ()  // idk if slotName needed
    {
        if (GameManager.Instance == null) // || SettingsManager.Instance = null)
            return false;

        gameLevel = 0; //GameManager.Instance. // I couldn't find where the current level was saved
        gold = GameManager.Instance.Gold;
        playerUnitRoster = GameManager.Instance.PlayerRoster;

        // uncomment when settings manager is in the game
        masterVolume = 0; // SettingsManager.Instance.masterVolume
        uiVolume = 0; // SettingsManager.Instance.uiVolume
        musicVolume = 0; // SettingsManager.Instance.musicVolume
        gameplayVolume = 0; // SettingsManager.Instance.gameplayVolume
        zoomLevel = 0; // SettingsManager.Instance.zoomLevel
        gameSpeed = 0; // SettingsManager.Instance.gameSpeed

        hasData = true;
        return true;
    }

    public bool LoadGame () // dosen't need slotName because SaveGameManager will handle the rest of it
    {
        if (!hasData || GameManager.Instance == null) // || SettingsManager.Instance = null)
            return false; 

        // GameManager.Instance.currentLevel = gameLevel // idk the implementation, edit as needed
        GameManager.Instance.Gold = gold; // not using add gold bc of steam achievements
        GameManager.Instance.loadPlayerRoster(playerUnitRoster);

        // SettingsManager.Instance.masterVolume = masterVolume;
        // SettingsManager.Instance.uiVolume = uiVolume;
        // SettingsManager.Instance.musicVolume = musicVolume;
        // SettingsManager.Instance.gameplayVolume = gameplayVolume;

        // SettingsManager.Instance.zoomLevel = zoomLevel;
        // SettingsManager.Instance.gameSpeed = gameSpeed;
    
        return true;
    }

    public enum SlotName
    {
        Slot1,
        Slot2,
        Slot3,
        Slot4
    }
}