using UnityEngine;
using System.Collections.Generic;

public class CampUIManager : MonoBehaviour
{
    public static CampUIManager Instance;

    [Header("Menus")]
    public GameObject shopMenu;
    public GameObject armyMenu;
    public GameObject supportMenu;
    public GameObject optionsMenu;

    private Dictionary<CampMenuType, GameObject> menuMap;
    private GameObject currentOpenMenu;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        menuMap = new Dictionary<CampMenuType, GameObject>
        {
            { CampMenuType.Shop, shopMenu },
            { CampMenuType.Army, armyMenu },
            { CampMenuType.Support, supportMenu },
            { CampMenuType.Options, optionsMenu }
        };

        foreach (var menu in menuMap.Values)
        {
            menu.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsMenuOpen())
        {
            CloseCurrentMenu();
        }
    }

    public void OpenMenu(CampMenuType type)
    {
        if(currentOpenMenu != null) return; // alr in menu

        if(menuMap.TryGetValue(type, out GameObject menu))
        {
            menu.SetActive(true);
            currentOpenMenu = menu;

            if (menu.TryGetComponent(out ICampMenu campMenu))
                campMenu.Open();

            CampInputBlocker.SetBlocked(true);
        }
    }

    public void CloseCurrentMenu()
    {
        if(currentOpenMenu == null) return;

        if(currentOpenMenu.TryGetComponent(out ICampMenu campMenu))
            campMenu.Close();

        currentOpenMenu.SetActive(false);
        currentOpenMenu = null;

        CampInputBlocker.SetBlocked(false);  
    }

    public bool IsMenuOpen() => currentOpenMenu != null;
}