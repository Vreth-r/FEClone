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
                if (gameMenu.MenuID != MenuType.MainMenu)
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


    // Overload for ActionMenu
    public void OpenMenu(MenuType type, UnitMovement unit, Vector3 worldPos)
    {
        if (menuMap.TryGetValue(type, out IGameMenu menu))
        {
            var aMenu = menu as ActionMenu;
            CloseCurrentMenu();
            aMenu.Open(unit, worldPos);
            currentMenu = menu;
        }
    }

    //Overload for StatsMenu
    public void OpenMenu(MenuType type, Unit unit)
    {
        if (menuMap.TryGetValue(type, out IGameMenu menu))
        {
            var sMenu = menu as StatsMenu;
            CloseCurrentMenu();
            sMenu.Open(unit);
            currentMenu = menu;
        }
    }

    public void CloseCurrentMenu()
    {
        if (currentMenu != null && currentMenu.IsOpen && currentMenu.escapable)
        {
            currentMenu.Close();
            currentMenu = null;
        }
    }

    public void CloseMenu(MenuType type)
    {
        if (currentMenu != null && currentMenu.MenuID == type && currentMenu.IsOpen)
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