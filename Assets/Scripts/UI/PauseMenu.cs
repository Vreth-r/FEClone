using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour, IGameMenu
{
    public MenuType MenuID => MenuType.PauseMenu;
    public bool IsOpen { get; private set; }
    public bool escapable { get; private set; }

    public Button resumeButton;
    public Button saveButton;
    public Button mainMenuButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(OnResume);
        saveButton.onClick.AddListener(OnSave);
        mainMenuButton.onClick.AddListener(OnMainMenu);
        escapable = true;
        IsOpen = false;
    }

    public void Open()
    {
        Time.timeScale = 0f;
        gameObject.SetActive(true);
        IsOpen = true;
    }

    public void Close()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        IsOpen = false;
    }

    private void OnResume()
    {
        UIManager.Instance.CloseMenu(MenuType.PauseMenu);
    }

    private void OnMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // had originally designed this a quit button, need to refactor contents
    private void OnSave()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}