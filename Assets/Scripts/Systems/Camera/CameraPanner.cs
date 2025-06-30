using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraPanner : MonoBehaviour
{
    [Header("References")]
    public Grid grid;
    public Tilemap tilemap;

    [Header("Cursor")]
    public CursorController cursor;

    [Header("Camera Settings")]
    public float cameraSpeed = 1f;

    private Camera cam;
    private Vector3 minCameraPos;
    private Vector3 maxCameraPos;

    private float halfCamHeight;
    private float halfCamWidth;

    void Start()
    {
        cam = Camera.main;

        // get cam dimensions
        halfCamHeight = cam.orthographicSize;
        halfCamWidth = halfCamHeight * cam.aspect;
    }

    public void LoadGridBounds()
    {
        Bounds mapBounds = tilemap.localBounds;
        // clamp
        minCameraPos = mapBounds.min + new Vector3(halfCamWidth, halfCamHeight, 0f);
        maxCameraPos = mapBounds.max - new Vector3(halfCamWidth, halfCamHeight, 0f);
    }

    void LateUpdate()
    {
        // convert grid cursor position to world space
        Vector3 targetWorldPos = grid.CellToWorld(cursor.GetCursorGridPosition()) + grid.cellSize / 2f;

        // maintain og z 
        targetWorldPos.z = transform.position.z;

        // clamp the targetpos within cam bounds
        targetWorldPos.x = Mathf.Clamp(targetWorldPos.x, minCameraPos.x, maxCameraPos.x);
        targetWorldPos.y = Mathf.Clamp(targetWorldPos.y, minCameraPos.y, maxCameraPos.y);

        // smoove cam movement
        transform.position = Vector3.Lerp(transform.position, targetWorldPos, cameraSpeed * Time.deltaTime);
    }

}

