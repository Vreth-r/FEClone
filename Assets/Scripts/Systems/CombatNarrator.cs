using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class CombatNarrator : MonoBehaviour
{
    public TextMeshProUGUI narrationText;

    public IEnumerator ShowMessage(string message)
    {
        narrationText.text = message;
        yield return new WaitForSeconds(0f);
    }

    public IEnumerator ShowMessageAndClear(string message, float duration = 0.5f)
    {
        narrationText.text = message;
        yield return new WaitForSeconds(duration);
        narrationText.text = "";
    }
}