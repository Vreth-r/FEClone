using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, IGameMenu
{
    public MenuType MenuID => MenuType.MainMenu;
    public bool IsOpen { get; private set; }

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
        SceneManager.LoadScene("CampScene");
    }

    // These are mapped in the editor for each button listener
    public void ContinueGame()
    {
        Debug.Log("Loading save...");
        if (!SaveSystem.SaveExists(0)) return;

        SaveData data = SaveSystem.LoadGame(0);
        if (data != null)
        {
            SceneManager.LoadScene(data.sceneName);
            // put the peeps in their positions
        }
        else
        {
            Debug.Log("No Save Found");
        }
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
/*
Code to actually save and load to a slot, need to figure out where to actually put this

public void LoadSlot(int slot)
{
    if (SaveSystem.SaveExists(slot))
    {
        SaveData data = SaveSystem.LoadGame(slot);
        SceneManager.LoadScene(data.sceneName);
    }
}

// I wrote this one with a game manager script in mind cause where the fuck else is shit like
// currency and convoy gonna go
public void SaveSlot(int slot)
{
    var save = new SaveData
    {
        sceneName = SceneManager.GetActiveScene().name,
        playerPosition = GameManager.Instance.Player.transform.position,
        gold = GameManager.Instance.Gold,
        recruitedUnits = GameManager.Instance.GetRecruitedUnitIDs(),
        timestamp = System.DateTime.Now.ToString("g")
    };

    SaveSystem.SaveGame(save, slot);
}
*/