using UnityEngine;

public class ShopMenu : MonoBehaviour, IGameMenu
{
    public MenuType MenuID => MenuType.ShopMenu;
    public bool IsOpen { get; private set; }

    public void Open()
    {
        gameObject.SetActive(true);
        IsOpen = true;
        Debug.Log("Shop Menu opened");
    }

    public void Close()
    {
        gameObject.SetActive(false);
        IsOpen = false;
        Debug.Log("Shop menu closed");
    }

    // button hooks go here
}
