using UnityEngine;

public class ShopMenu : MonoBehaviour, IGameMenu
{
    public MenuType MenuID => MenuType.ShopMenu;
    public bool IsOpen { get; private set; }
    public bool escapable { get; private set; }

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        escapable = true;
        IsOpen = false;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        IsOpen = true;
        Debug.Log("ShopMenu.cs: Shop Menu opened");
    }

    public void Close()
    {
        gameObject.SetActive(false);
        IsOpen = false;
        Debug.Log("ShopMenu.cs: Shop menu closed");
    }

    // button hooks go here
}
