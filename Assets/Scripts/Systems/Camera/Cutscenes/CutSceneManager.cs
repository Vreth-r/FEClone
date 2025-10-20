using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Yarn.Unity;
using System.Threading;
using TMPro;
using UnityEngine.UI;

// Cutscene Manager! 
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }
    public CameraPanner cameraPanner; // camera panner ref to do things with
    public CutsceneDescriptionData testCutscene; // Testing only

    [Header("Fade Elements")]
    public Image fadeImage;
    public TextMeshProUGUI fadeText;

    public event Action<string> CutsceneCue;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
        }
        if (fadeText != null)
        {
            fadeText.alpha = 0;
        }
    }

    void Start() // testing only
    {
        if (testCutscene) StartCoroutine(TestRun());
    }

    public void FireEvent(string eventName)
    {
        CutsceneCue?.Invoke(eventName);
    }

    public IEnumerator TestRun() // testing only
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(PlayCutscene(testCutscene));
    }

    // play entire cutscene
    public IEnumerator PlayCutscene(CutsceneDescriptionData data)
    {
        cameraPanner.SetInCutscene(true); // stop camera panner from doing regular behaviour 
        // itterate through all steps in cutscene description
        foreach (var step in data.steps)
        {
            List<Coroutine> eventCoroutines = new List<Coroutine>(); // to run coroutines in parallel 
            foreach (var e in step.events) // add all events in steps to coroutines list and start them
            {
                eventCoroutines.Add(StartCoroutine(RunEvent(e)));
            }

            foreach (var c in eventCoroutines) // Wait for all in this step to complete
            {
                yield return c;
            }
        }
        cameraPanner.SetInCutscene(false); // restart regular behaviour 
    }

    // coroutine to run a cutscene event
    private IEnumerator RunEvent(CutsceneEvent e)
    {
        yield return new WaitForSeconds(e.delay); // generic delay

        // call different coroutines based on event type
        switch (e.type)
        {
            case CutsceneEventType.PanToLocation:
                yield return cameraPanner.PanToLocation(e.vector3Param, e.floatParam1);
                break;
            case CutsceneEventType.PanToUnit:
                yield return cameraPanner.PanToUnit(e.stringParam1, e.floatParam1);
                break;
            case CutsceneEventType.CameraShake:
                yield return cameraPanner.ShakeCamera(e.floatParam1, e.floatParam2);
                break;
            case CutsceneEventType.UnitJump:
                yield return UnitManager.Instance.JumpUnit(e.stringParam1, e.floatParam1);
                break;
            case CutsceneEventType.UnitMoveToPos:
                yield return UnitManager.Instance.MoveUnitTo(e.stringParam1, e.vector2IntParam);
                break;
            case CutsceneEventType.UnitEmote:
                yield return UnitManager.Instance.EmoteUnit(e.stringParam1, e.stringParam2, e.floatParam1);
                break;
            case CutsceneEventType.Wait:
                yield return new WaitForSeconds(e.floatParam1);
                break;
        }
    }

    public IEnumerator FadeIn(float duration)
    {
        if (fadeImage == null)
        {
            yield break;
        }

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    public IEnumerator FadeOut(float duration)
    {
        if (fadeImage == null)
        {
            yield break;
        }

        float t = 0;
        Color c = fadeImage.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - t / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // hide text if any
        if (fadeText != null)
            fadeText.alpha = 0;
    }

    public IEnumerator ShowText(string text, float duration)
    {
        fadeText.text = text;

        // fade in
        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            fadeText.alpha = Mathf.Lerp(0, 1, t / 0.5f);
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        // fade out
        t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            fadeText.alpha = Mathf.Lerp(1, 0, t / 0.5f);
            yield return null;
        }
    }

    public YarnTask YarnCoroutine(IEnumerator coroutine)
    {
        var tcs = new YarnTaskCompletionSource();
        StartCoroutine(WrapCoroutine(coroutine, tcs));
        return tcs.Task;
    }

    private IEnumerator WrapCoroutine(IEnumerator coroutine, YarnTaskCompletionSource tcs)
    {
        yield return coroutine;
        tcs.TrySetResult();
    }

    public IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
