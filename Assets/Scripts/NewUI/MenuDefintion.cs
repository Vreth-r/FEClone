using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class MenuDefinition
{
    public string id;
    public VisualTreeAsset uxml;
    public StyleSheet stylesheet;
    public UIMenuType menuType;
}

public enum UIMenuType
{
    Screen, // clears stack
    Overlay, // Stacks on top
    Modal // blocks input behind
}