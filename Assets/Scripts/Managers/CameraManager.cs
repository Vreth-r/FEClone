using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    private Camera cameraComponent;
    private CameraPanner cameraPanner;
    private CampCameraPanner campCameraPanner;

    private float minZoom = 2.25f;
    private float maxZoom = 7.75f;
    private float currentZoom = 5f;
    private float currentZoomPercent = 5f;
    void Awake()
    {
        if (Instance == null)
            Instance = this;

        Debug.Log("Instance");
    }
    void Start()
    {
        cameraComponent = gameObject.GetComponent<Camera>(); // idk might need this but prob not
    }

    public void UpdateCameraZoom (float percent)
    {
        currentZoomPercent = percent;
        currentZoom = Mathf.Lerp(minZoom, maxZoom, currentZoomPercent);
        //cameraComponent.orthographicSize = currentZoom;
        if (cameraPanner != null)
            cameraPanner.SetZoom(currentZoom);
        else if (campCameraPanner != null)
            campCameraPanner.SetZoom(currentZoom);
        else
            Debug.Log("No camera panner registered");
    }

    public void SetCameraPanner (CameraPanner cameraPanner)
    {
        this.cameraPanner = cameraPanner;
        campCameraPanner = null;
    }
    public void SetCampCameraPanner (CampCameraPanner campCameraPanner)
    {
        this.campCameraPanner = campCameraPanner;
        cameraPanner = null;
    }
}
