using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tactics RPG/Portrait Library")]
public class CharacterPortraitLibrary : ScriptableObject
{
    [System.Serializable]
    public class PortraitEntry
    {
        public string characterName;
        public string expression;
        public Sprite sprite;
    }

    public List<PortraitEntry> portraits;

    public Sprite GetPortrait(string characterName, string expression)
    {
        foreach (var entry in portraits)
        {
            if (entry.characterName == characterName && entry.expression == expression)
                return entry.sprite;
        }

        return null;
    }
}