using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public GameObject loadingScreenPrefab;
    public static LoadingScreenManager Instance;

    private GameObject currentScreen;
    private CanvasGroup canvasGroup;
    private Slider progressBar;
    private TextMeshProUGUI loadingText;

    private void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (loadingScreenPrefab == null)
        {
            Debug.LogError("Loading screen prefab not assigned.");
            yield break;
        }

        // Create screen
        currentScreen = Instantiate(loadingScreenPrefab);
        DontDestroyOnLoad(currentScreen);

        canvasGroup = currentScreen.GetComponentInChildren<CanvasGroup>();
        progressBar = currentScreen.GetComponentInChildren<Slider>();
        loadingText = currentScreen.GetComponentInChildren<TextMeshProUGUI>();

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;

        // Fade in
        yield return FadeCanvas(0f, 1f, 0.5f);

        // Begin scene load
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Update progress bar
        float targetProgress = 0f;

        while (!asyncLoad.isDone)
        {
            // apparently, unity only loads to 0.9 b4 waiting for the scene to activate
            targetProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            // smooth fill
            progressBar.value = Mathf.Lerp(progressBar.value, targetProgress, Time.deltaTime * 5f);

            // text update
            int percent = Mathf.RoundToInt(progressBar.value * 100f);
            loadingText.text = $"Loading... {percent}%";

            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
        // Set progress to full and display a short message
        progressBar.value = 1f;
        loadingText.text = $"Loading Complete";

        asyncLoad.allowSceneActivation = true;

        // Wait one frame for scene to switch
        yield return null;

        // Fade out
        yield return FadeCanvas(1f, 0f, 0.5f);

        Destroy(currentScreen);
    }

    private IEnumerator FadeCanvas(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / duration);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
