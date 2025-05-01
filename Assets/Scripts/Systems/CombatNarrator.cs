using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class CombatNarrator : MonoBehaviour
{
    public TextMeshProUGUI narrationText;

    public IEnumerator ShowMessage(string message, float duration = 1.2f)
    {
        narrationText.text = message;
        yield return new WaitForSeconds(duration);
        narrationText.text = "";
    }
}