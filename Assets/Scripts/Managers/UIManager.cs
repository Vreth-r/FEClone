using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [System.Serializable]
    public struct MenuPrefabEntry
    {
        public MenuType type;
        public GameObject prefab;
    }

    [Header("Menu Prefabs")]
    public List<MenuPrefabEntry> menuPrefabs;

    private Dictionary<MenuType, IGameMenu> menuMap = new();
    private Dictionary<MenuType, GameObject> prefabMap = new();
    private IGameMenu currentMenu; // might change to stack later for multi menuing.

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Close each menu when starting
        foreach (var menu in menuPrefabs)
        {
            if (menu.prefab.TryGetComponent<IGameMenu>(out var menuComponent))
            {
                prefabMap[menu.type] = menu.prefab;
            }
            else
            {
                Debug.LogError($"UIManager: Prefab for {menu.type} does not implement IGameMenu");
            }
        }
    }

    // this is designed to reuse the same menu instance, so everything gets instantiated once and only once.
    private IGameMenu GetOrCreateMenu(MenuType type)
    {
        if (menuMap.TryGetValue(type, out var menu)) return menu;

        if (!prefabMap.TryGetValue(type, out var prefab))
        {
            Debug.LogError($"UIManager: No prefab found for menu type: {type}");
            return null;
        }

        GameObject instance = Instantiate(prefab);
        instance.SetActive(false);

        if (instance.TryGetComponent<IGameMenu>(out var newMenu))
        {
            menuMap[type] = newMenu;
            return newMenu;
        }

        Debug.LogError($"UIManager: Instantiated menu does not implement IGameMenu: {type}");
        Destroy(instance);
        return null;

    }

    // Called externally from other classes, like CampPlayerController
    public void OpenMenu(MenuType type)
    {
        var menu = GetOrCreateMenu(type);
        if (menu == null) return;

        CloseCurrentMenu();
        menu.Open();
        currentMenu = menu;
    }

    // Overload for ActionMenu
    public void OpenMenu(MenuType type, UnitMovement unit, Vector3 worldPos)
    {

        var menu = GetOrCreateMenu(type);
        if (menu is ActionMenu aMenu)
        {
            CloseCurrentMenu();
            aMenu.Open(unit, worldPos);
            currentMenu = menu;
        }
        else
        {
            Debug.LogWarning($"UIManager: Menu {type} is not an ActionMenu");
        }
    }

    //Overload for StatsMenu
    public void OpenMenu(MenuType type, Unit unit)
    {
        var menu = GetOrCreateMenu(type);
        if (menu is StatsMenu sMenu)
        {
            CloseCurrentMenu();
            sMenu.Open(unit);
            currentMenu = sMenu;
        }
        else
        {
            Debug.LogWarning($"UIManager: Menu {type} is not a StatsMenu.");
        }
    }

    public void CloseCurrentMenu()
    {
        if (currentMenu != null && currentMenu.IsOpen && currentMenu.escapable)
        {
            currentMenu.Close();
            currentMenu = null;
            CampInputBlocker.SetBlocked(false);
        }
    }

    public void WipeCurrentMenu()
    {
        currentMenu = null;
    }

    // why...do i have this?
    public void CloseMenu(MenuType type)
    {
        if (menuMap.TryGetValue(type, out var menu) && menu.IsOpen)
        {
            menu.Close();
            if (currentMenu == menu) currentMenu = null;
        }
    }

    public MenuType GetCurrentMenuType()
    {
        if (currentMenu != null)
        {
            return currentMenu.MenuID;
        }
        return MenuType.None;
    }

    public bool IsMenuOpen() => currentMenu != null && currentMenu.IsOpen;

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsMenuOpen())
            {
                CloseCurrentMenu();
            }
            else
            {
                OpenMenu(MenuType.PauseMenu);
            }
        }
    }
}