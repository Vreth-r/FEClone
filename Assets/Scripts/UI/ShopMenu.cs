using UnityEngine;

public class ShopMenu : MonoBehaviour, ICampMenu
{
    public void Open()
    {
        Debug.Log("Shop Menu opened");
    }

    public void Close()
    {
        Debug.Log("Shop menu close");
    }
}