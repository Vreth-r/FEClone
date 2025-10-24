using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using Cysharp.Threading.Tasks;

public class CombatNarrator : MonoBehaviour
{
    public TextMeshProUGUI narrationText;

    public IEnumerator ShowMessage(string message)
    {
        narrationText.text = message;
        yield return new WaitForSeconds(0f);
    }

    public async UniTask ShowMessageAsync(string message)
    {
        await ShowMessage(message).ToUniTask();
    }

    public IEnumerator ShowMessageAndClear(string message, float duration = 0.5f)
    {
        narrationText.text = message;
        yield return new WaitForSeconds(duration);
        narrationText.text = "";
    }

    public async UniTask ShowMessageAndClearAsync(string message, float duration = 0.5f)
    {
        narrationText.text = message;
        await UniTask.Delay((int)duration * 1000);
        narrationText.text = "";
    }
}