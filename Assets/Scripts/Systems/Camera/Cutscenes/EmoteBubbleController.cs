using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class EmoteBubbleController : MonoBehaviour
{
    public GameObject go; // self? (documenting this later on and i dont remember why this is so)
    public TextMeshProUGUI textMeshPro; // text ref

    public void Awake()
    {
        go.SetActive(false); // deactivate before everything starts
    }
    public IEnumerator ShowEmote(string emoteText, float duration) // show given emote for amount of time
    {
        go.SetActive(true);
        textMeshPro.text = emoteText;
        yield return new WaitForSeconds(duration);
        go.SetActive(false);
    }
}
