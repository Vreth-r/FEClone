using UnityEngine;
using UnityEngine.Tilemaps;

/* READ THIS
so the camera follows the cursor pretty closely in the ds titles of fe, so i might just copy that
when selecting a character, you move them with the cursor, and when you click move, the camera goes to the 
center point between the starting position and the end position while the character moves, it then centers on the character
and the action menu pops up
*/

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

