using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class GlobalLightController : MonoBehaviour
{
    private Light2D light2D;

    [Header("Light Levels")]
    [SerializeField] private float on = 1f;
    [SerializeField] private float off = 0f;
    [SerializeField] private float dim = 0.1f;
    [SerializeField] private float dark = 0.01f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f; // seconds

    private Coroutine fadeRoutine;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        CutsceneManager.Instance.CutsceneCue += HandleCutsceneCue;
    }

    private void HandleCutsceneCue(string eventName)
    {
        float targetIntensity = light2D.intensity;

        switch (eventName)
        {
            case "globallighton":
                targetIntensity = on;
                break;
            case "globallightoff":
                targetIntensity = off;
                break;
            case "globallightdim":
                targetIntensity = dim;
                break;
            case "globallightdark":
                targetIntensity = dark;
                break;
            default:
                return;
        }

        // Stop any running fade before starting a new one
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeToIntensity(targetIntensity));
    }

    private IEnumerator FadeToIntensity(float target)
    {
        float start = light2D.intensity;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            light2D.intensity = Mathf.Lerp(start, target, time / fadeDuration);
            yield return null;
        }

        light2D.intensity = target;
        fadeRoutine = null;
    }

    private void OnDestroy()
    {
        if (CutsceneManager.Instance != null)
            CutsceneManager.Instance.CutsceneCue -= HandleCutsceneCue;
    }
}
