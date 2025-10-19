using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    private Light2D light2D;
    public float flickerSpeed = 5f;
    public float intensityVariation = 0.3f;
    private float baseIntensity;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        baseIntensity = light2D.intensity;
    }

    void Update()
    {
        // THE VERY SAME ALGORITHM THAT MINECRAFT USES TO CREATE NATURAL LOOKING TERRAIN
        light2D.intensity = baseIntensity + Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) * intensityVariation;
    }
}
