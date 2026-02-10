using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuTheSecond : UIMenuBase
{
    private Button resumeButton;
    private Button saveButton;
    private Button quitButton;
    private Button settingsButton;

    protected override void OnCreate()
    {
        resumeButton = Root.Q<Button>("ResumeButton");
        saveButton = Root.Q<Button>("SaveButton");
        quitButton = Root.Q<Button>("QuitButton");
        settingsButton = Root.Q<Button>("SettingsButton");

        resumeButton.clicked += Resume;
        saveButton.clicked += Save;
        quitButton.clicked += Quit;
        settingsButton.clicked += Settings;
    }

    public override void OnOpen()
    {
        Time.timeScale = 0f;
    }

    public override void OnClose()
    {
        Time.timeScale = 1f;
    }

    private void Resume()
    {
        manager.CloseTopMenu();
    }

    private void Save()
    {
        SaveGameManager.Instance.SaveGame(SaveGameData.SlotName.Slot1);
        Debug.Log("Game Saved (placeholder)");
    }

    private void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void Settings ()
    {
        UIManagerTheSecond.Instance.OpenMenu<SettingsMenu>("SettingsMenu");
    }
}
