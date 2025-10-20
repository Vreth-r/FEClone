using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    private Light2D light2D;
    public float flickerSpeed = 5f;
    public float intensityVariation = 0.3f;
    private float baseIntensity;
    public bool working = true;
    private float randomOffset;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        baseIntensity = light2D.intensity;
        CutsceneManager.Instance.CutsceneCue += HandleCutsceneCue;
        randomOffset = Random.Range(0f, 1000f);
    }

    void Update()
    {
        if (!working) return;
        // THE VERY SAME ALGORITHM THAT MINECRAFT USES TO CREATE NATURAL LOOKING TERRAIN
        light2D.intensity = baseIntensity + Mathf.PerlinNoise(Time.time * flickerSpeed + randomOffset, 0f) * intensityVariation;
    }

    private void HandleCutsceneCue(string eventName)
    {
        if (eventName == "timestop")
        {
            working = false;
        }
        else if (eventName == "timestart")
        {
            working = true;
        }
    }
}
