using UnityEngine;

public class BarracksMenu : MonoBehaviour, ICampMenu
{
    public void Open()
    {
        Debug.Log("Barracks Menu opened");
    }

    public void Close()
    {
        Debug.Log("Barracks menu close");
    }
}