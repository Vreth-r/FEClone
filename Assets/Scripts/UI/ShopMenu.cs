using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour, IGameMenu
{
    public MenuType MenuID => MenuType.ShopMenu;
    public bool IsOpen { get; private set; }
    public bool escapable { get; private set; }

    public Button enterButton;
    public Button talkButton;
    public Button exitButton;

    public void Awake()
    {
        escapable = true;
        IsOpen = false;
        talkButton.onClick.AddListener(OnTalk);
        exitButton.onClick.AddListener(OnExit);
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

    private void OnTalk()
    {
        throw new System.NotImplementedException();
        //GameManager.Instance.MasterYarnRunner.StartDialogue("ShopChat");
    }
    private void OnExit()
    {
        Close();
        //CampInputBlocker.SetBlocked(false);
    }
}
