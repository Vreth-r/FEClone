using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image foregroundBar;
    [SerializeField] private float animationSpeed = 5f;

    private float currentPercent = 1f;

    public void SetHealth(float current, float max)
    {
        currentPercent = Mathf.Clamp01(current / max);
    }

    private void Update()
    {
        if(foregroundBar != null)
        {
            float displayed = foregroundBar.fillAmount;
            float newFill = Mathf.Lerp(displayed, currentPercent, Time.deltaTime * animationSpeed);
            foregroundBar.fillAmount = newFill;
        }
    }

    public void InstantFill(float current, float max)
    {
        currentPercent = Mathf.Clamp01(current / max);
        if (foregroundBar != null) foregroundBar.fillAmount = currentPercent;
    }
}
