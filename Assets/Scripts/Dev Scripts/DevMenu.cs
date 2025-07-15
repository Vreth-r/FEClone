using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DevMenu : MonoBehaviour
{
    public Button encodeMap;
    public Button loadMap;

    public MapEncoder mapEncoder;
    public MapLoader mapLoader;

    public TextMeshProUGUI controlContextText;

    public TextMeshProUGUI uiMenuCountText;

    public void Awake()
    {
        encodeMap.onClick.AddListener(OnEncodeMap);
        //loadMap.onClick.AddListener(OnLoadMap);
    }

    public void OnEncodeMap()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "CampScene")
        {
            Debug.Log("DevMenu (OnEncodeMap): Can't invoke that in this scene");
            return;
        }

        Debug.Log("DevMenu (OnEncodeMap): Invoking...");
        mapEncoder.ExportCurrentMap();
    }

    public void OnLoadMap()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "CampScene")
        {
            Debug.Log("DevMenu (OnLoadMap): Can't invoke that in this scene");
            return;
        }

        Debug.Log("DevMenu (OnLoadMap): Invoking...");
        mapLoader.LoadFromField();
    }

    public void Update()
    {
        if (ControlsManager.Instance != null) controlContextText.text = ControlsManager.Instance.CurrentContext.ToString();
        if (UIManager.Instance != null) uiMenuCountText.text = UIManager.Instance.menuStack.Count.ToString();
    }
}