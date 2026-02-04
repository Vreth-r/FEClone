using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneAnimationController : MonoBehaviour
{
    public static SceneAnimationController Instance; // singleton for now

    [System.Serializable]
    public class AnimEntry
    {
        public string objectName;
        public Animator animator;
    }

    public List<AnimEntry> animatorList;
    private Dictionary<string, Animator> animators = new Dictionary<string, Animator>();
    private Dictionary<string, Coroutine> runningCoroutines = new Dictionary<string, Coroutine>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        RegisterAllSceneAnimators();
    }

    // Register animators by name later i think i need this
    public void RegisterAnimator(string key, Animator animator)
    {
        if (!animators.ContainsKey(key))
        {
            animators.Add(key, animator);
            animatorList.Add(new AnimEntry { objectName = key, animator = animator });
            Debug.Log($"Registered animator {key}");
        }
    }

    /// <summary>Scans the entire active scene for any objects with Animator components and registers them.</summary>
    public void RegisterAllSceneAnimators()
    {
        Animator[] foundAnimators = FindObjectsByType<Animator>(FindObjectsSortMode.None);

        foreach (var animator in foundAnimators)
        {
            if(animator.gameObject.name == "LoadingScreen(Clone)")
            {
                continue; // skip the loading screen
            }
            string key = animator.gameObject.name;

            if (!animators.ContainsKey(key))
            {
                animators.Add(key, animator);
                animatorList.Add(new AnimEntry { objectName = key, animator = animator });
                Debug.Log($"Registered animator: {key}");
            }
        }
    }

    public void RefreshAnimators()
    {
        animators.Clear();
        animatorList.Clear();
        RegisterAllSceneAnimators();
    }

    /// <summary>Play an animation immediately. Optionally loop it.</summary>
    public void PlayAnimation(string key, string animName)
    {
        if (!animators.TryGetValue(key, out var animator))
        {
            Debug.LogWarning($"Animator '{key}' not found!");
            return;
        }

        // Stop any currently running auto-stop coroutine for this object
        if (runningCoroutines.ContainsKey(key))
        {
            StopCoroutine(runningCoroutines[key]);
            runningCoroutines.Remove(key);
        }

        animator.Play(animName);
    }

    /// <summary>Stop a specific animation loop manually.</summary>
    public void StopAnimation(string key, string animName)
    {
        if (!animators.TryGetValue(key, out var animator))
            return;

        // Stop any running coroutine
        if (runningCoroutines.ContainsKey(key))
        {
            StopCoroutine(runningCoroutines[key]);
            runningCoroutines.Remove(key);
        }

        animator.SetBool(animName + "_Loop", false);
        animator.Play("Idle"); // default idle
    }

    private IEnumerator AutoStopAnimation(string key, string animName, float duration, bool loop)
    {
        yield return new WaitForSeconds(duration);

        if (animators.TryGetValue(key, out var animator))
        {
            if (loop)
                animator.SetBool(animName + "_Loop", false);

            animator.Play("idle"); // default idle
        }

        runningCoroutines.Remove(key);
    }

    // 0 for pause 1 for full speed
    public void SetAllAnimatorSpeeds(float time)
    {
        foreach (var entry in animatorList)
        {
            Debug.Log(entry.objectName);
            entry.animator.speed = time;
        }
    } 

    public void SetAnimatorSpeed(string key, float time)
    {
        if (!animators.TryGetValue(key, out var animator))
        {
            Debug.LogWarning($"Animator '{key}' not found!");
            return;
        }

        animator.speed = time;
    }
}

