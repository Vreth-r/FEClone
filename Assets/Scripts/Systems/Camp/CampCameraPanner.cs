using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CampCameraPanner : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Smoothing")]
    [Range(0.01f, 1f)]
    public float smoothTime = 0.15f;

    [Header("Camera Bounds")]
    public BoxCollider2D boundsCollider;

    private Camera cam;
    private Vector3 velocity = Vector3.zero;

    private float camHalfHeight;
    private float camHalfWidth;

    private Bounds bounds;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    private void Start()
    {
        CalculateCameraExtents();
        UpdateBounds();
    }

    private void LateUpdate()
    {
        if (target == null || boundsCollider == null)
            return;

        UpdateBounds();

        Vector3 targetPos = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        Vector3 smoothPos = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );

        smoothPos.x = Mathf.Clamp(
            smoothPos.x,
            bounds.min.x + camHalfWidth,
            bounds.max.x - camHalfWidth
        );

        smoothPos.y = Mathf.Clamp(
            smoothPos.y,
            bounds.min.y + camHalfHeight,
            bounds.max.y - camHalfHeight
        );

        transform.position = smoothPos;
    }

    private void CalculateCameraExtents()
    {
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    private void UpdateBounds()
    {
        bounds = boundsCollider.bounds;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (boundsCollider == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            boundsCollider.bounds.center,
            boundsCollider.bounds.size
        );
    }
#endif
}
