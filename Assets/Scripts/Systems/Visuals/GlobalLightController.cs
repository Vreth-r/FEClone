using UnityEngine;
using UnityEngine.Rendering.Universal;
public class GlobalLightController : MonoBehaviour
{
    private Light2D light2D;
    private float on = 1f;
    private float off = 0f;
    private float dim = 0.1f;
    private float dark = 0.01f;
    void Start()
    {
        light2D = GetComponent<Light2D>();
        CutsceneManager.Instance.CutsceneCue += HandleCutsceneCue;
    }

    private void HandleCutsceneCue(string eventName)
    {
        switch (eventName)
        {
            case "globallighton":
                light2D.intensity = on;
                break;
            case "globallightoff":
                light2D.intensity = off;
                break;
            case "globallightdim":
                light2D.intensity = dim;
                break;
            case "globallightdark":
                light2D.intensity = dark;
                break;
        }
    }
}
