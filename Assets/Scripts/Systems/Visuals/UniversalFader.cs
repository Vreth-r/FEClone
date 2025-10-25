using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasGroup))]
public class UniversalFader : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Coroutine currentFade;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetInstant(float alpha)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = alpha > 0.01f;
    }

    public void FadeIn(float duration = 1f)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeRoutine(1f, duration));
    }

    public void FadeOut(float duration = 1f)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeRoutine(0f, duration));
    }

    private IEnumerator FadeRoutine(float target, float duration)
    {
        float start = canvasGroup.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }

        canvasGroup.alpha = target;
        canvasGroup.blocksRaycasts = target > 0.01f;
        currentFade = null;
    }

    public async UniTask FadeRoutineAsync(float target, float duration)
    {
        await FadeRoutine(target, duration).ToUniTask();
        await UniTask.WaitUntil(() => currentFade == null);
    }
}
