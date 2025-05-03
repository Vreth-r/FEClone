using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image foregroundBar;
    [SerializeField] private float animationSpeed = 5f;

    private float currentPercent = 1f;

    public void SetHealth(int current, int max)
    {
        currentPercent = Mathf.Clamp01((float)current / max); 
    }

    private void Update()
    {
        if(foregroundBar != null)
        {
            foregroundBar.fillAmount = Mathf.Lerp(foregroundBar.fillAmount, currentPercent, Time.deltaTime * animationSpeed);
        }
    }

    public void InstantFill(int current, int max)
    {
        currentPercent = Mathf.Clamp01((float)current / max);
        foregroundBar.fillAmount = currentPercent;
    }
}
