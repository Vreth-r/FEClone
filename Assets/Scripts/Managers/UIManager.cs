using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Dictionary<MenuType, IGameMenu> menuMap = new();
    private IGameMenu currentMenu; // might change to stack later

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // Close each menu when starting
        foreach (var menu in GetComponentsInChildren<IGameMenu>(true))
        {
            if (menu is IGameMenu gameMenu)
            {
                menuMap[gameMenu.MenuID] = gameMenu;
                gameMenu.Close();
            }
        }
    }

    public void OpenMenu(MenuType type)
    {
        if (menuMap.TryGetValue(type, out IGameMenu menu))
        {
            CloseCurrentMenu();
            menu.Open();
            currentMenu = menu;
        }
    }

    public void CloseCurrentMenu()
    {
        if (currentMenu != null && currentMenu.IsOpen)
        {
            currentMenu.Close();
            currentMenu = null;
        }
    }

    public bool IsMenuOpen() => currentMenu != null && currentMenu.IsOpen;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsMenuOpen())
        {
            CloseCurrentMenu();
        }
    }
}