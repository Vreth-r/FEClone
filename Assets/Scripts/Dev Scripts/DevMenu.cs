using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DevMenu : MonoBehaviour
{
    public Button encodeMap;

    public MapEncoder mapEncoder;

    public void Awake()
    {
        encodeMap.onClick.AddListener(OnEncodeMap);
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
}