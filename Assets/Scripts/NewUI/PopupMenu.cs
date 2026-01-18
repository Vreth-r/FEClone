using UnityEngine.UIElements;

public class PopupMenu : UIMenuBase, IMenuWithData<string>
{
    private Label message;

    protected override void OnCreate()
    {
        message = Root.Q<Label>("Message");
        Root.Q<Button>("Close").clicked += manager.CloseTopMenu;
    }

    public void SetData(string data)
    {
        message.text = data;
    }
}