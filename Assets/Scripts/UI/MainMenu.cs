using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, IGameMenu
{
    public MenuType MenuID => MenuType.MainMenu;
    public bool IsOpen { get; private set; }
    public bool escapable { get; private set; }

    public void Awake()
    {
        escapable = false;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        IsOpen = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        IsOpen = false;
    }

    public void NewGame()
    {
        Debug.Log("Starting New Game");
        // game data init goes here
        /*
        foreach (var data in roster.startingUnits)
        {
            UnitSpawner.SpawnUnitFromTemplate(data, spawnLocation);
        } */
        LoadingScreenManager.Instance.LoadScene("CampScene");
    }

    // These are mapped in the editor for each button listener
    public void ContinueGame()
    {
        Debug.Log("Loading save...");
        if (!SaveSystem.SaveExists(0))
        {
            Debug.LogWarning("No Save Data found for slot");
            return;
        }
        SaveSystem.LoadGame(0);
    }

    public void OpenOptions()
    {
        UIManager.Instance.OpenMenu(MenuType.OptionsMenu);
    }

    public void OpenLoadGame()
    {
        UIManager.Instance.OpenMenu(MenuType.LoadMenu);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}