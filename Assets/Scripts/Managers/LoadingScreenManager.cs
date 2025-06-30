using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using Newtonsoft.Json;

public class LoadingScreenManager : MonoBehaviour
{
    public GameObject loadingScreenPrefab;
    public static LoadingScreenManager Instance;

    private GameObject currentScreen;
    private CanvasGroup canvasGroup;
    private Slider progressBar;
    private TextMeshProUGUI loadingText;
    private TextMeshProUGUI tipText;
    public Dictionary<string, List<string>> tips = new();

    private void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadTips();
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(string mapID)
    {
        LoadScene("LevelTemplate", () =>
        {
            StartCoroutine(FinishLevelLoad(mapID));
        });
    }

    private IEnumerator FinishLevelLoad(string mapID)
    {
        yield return null; // wait a frame

        MapLoader loader = FindObjectOfType<MapLoader>(); // will make this a ref later maybe unsure i need to really diagram out all this code
        if (loader == null)
        {
            Debug.LogError("No MapLoader found in template scene");
            yield break;
        }

        loader.LoadMap(mapID);
    }

    public void LoadScene(string sceneName, System.Action onComplete = null)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, onComplete));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, System.Action onComplete)
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
        tipText = currentScreen.transform.Find("Background").transform.Find("TipText").GetComponent<TextMeshProUGUI>();

        GetRandomTip(); // sets tip text in method

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

        onComplete?.Invoke(); // run after scene loads

        // idk maybe move this somewhere else I just couldn't find where
        //GameObject.Find("GameManager").GetComponent<UIManager>().ClearMenuMap(); // resets spawned menus tracked by UI manager
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

    public void GetRandomTip()
    {
        if (tips != null && tips.Count > 0)
        {
            // Pick a random category
            List<string> categories = new List<string>(tips.Keys);
            string randomCategory = categories[Random.Range(0, categories.Count)]; // will change this later to make jokes rarer and actual helpful tips more common later

            // Pick a random tip from the category
            List<string> catTips = tips[randomCategory];
            if (catTips.Count > 0)
            {
                string randomTip = catTips[Random.Range(0, catTips.Count)];
                tipText.text = randomTip;
            }
        }
    }

    private void LoadTips()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "tips.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            tips = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
        }
        else
        {
            Debug.LogError("Tips file not found: " + filePath);
        }
    }
}
