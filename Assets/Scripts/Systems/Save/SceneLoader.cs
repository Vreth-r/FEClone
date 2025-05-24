using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static event Action OnSceneLoaded;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            OnSceneLoaded?.Invoke();
            OnSceneLoaded = null; // Clear after fire
        };
    }
}
