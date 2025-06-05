using UnityEngine;

// a class to put all the dev tools i need for debugging and encoding jsons

public class DevManager : MonoBehaviour
{
    public GameObject devMenu;
    private bool menuActive = false;

    public void Awake()
    {
        DontDestroyOnLoad(devMenu);
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (menuActive)
            {
                devMenu.SetActive(false);
                menuActive = false;
            }
            else
            {
                devMenu.SetActive(true);
                menuActive = true;
            }
        }
    }
}