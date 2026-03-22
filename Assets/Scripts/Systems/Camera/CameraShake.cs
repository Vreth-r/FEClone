using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private float shakeAmount = 0.2f;

    private bool isShaking;

    public static CameraShake Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private IEnumerator ShakeRoutine()
    {
        if (isShaking) yield return null;
        isShaking = true;

        Vector3 originalPos = transform.position;

        float elapsed = 0.0f;

        while(elapsed < shakeDuration)
        {
            float x = Random.Range(-1, 1) * shakeAmount;
            float y = Random.Range(-1, 1) * shakeAmount;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            
            yield return null;
        }

        transform.localPosition = originalPos;
        isShaking = false;
    }

    public void Shake()
    {
        StartCoroutine(ShakeRoutine());
    }
}
