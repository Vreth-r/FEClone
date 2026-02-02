using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    private Camera cameraComponent;

    float minZoom = 2.25f;
    float maxZoom = 7.75f;
    float currentZoom = 5f;
    void Awake()
    {
        if (Instance == null)
            Instance = this;

        Debug.Log("Instance");
    }
    void Start()
    {
        cameraComponent = gameObject.GetComponent<Camera>();
    }

    public void UpdateCameraZoom (float percent)
    {
        currentZoom = Mathf.Lerp(minZoom, maxZoom, percent);
        cameraComponent.orthographicSize = currentZoom;
    }
}
