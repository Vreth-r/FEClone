public interface IGameMenu
{
    void Open();
    void Close();
    bool IsOpen { get; }
    MenuType MenuID { get; }
}

public enum MenuType
{
    None,
    ActionMenu,
    ShopMenu,
    ArmyMenu,
    SupportMenu,
    StatMenu,
    OptionsMenu
    // add more if needed
}