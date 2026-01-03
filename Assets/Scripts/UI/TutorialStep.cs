using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Tutorial Step")]
public class TutorialStep : ScriptableObject
{
    [TextArea(3, 6)]
    public string bodyText;

    public string title;
    public Sprite illustration;

    public bool blockInput = true;
    public bool escapable = true;
}