/*
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class CameraPanner : MonoBehaviour
{
    [SerializeField] private float panSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float deadzoneRadius = 3.5f; // radius where the mouse wont move

    private Camera cam;
    private ControlsManager highlighter;
    private bool autoPanning = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
        highlighter = ControlsManager.Instance;
    }

    private void Update()
    {
        if (!autoPanning)
        {
            if (highlighter == null || !highlighter.EnableInput) return;

            Vector3Int targetCell = highlighter.LastHighlightedCell;
            Vector3 targetWorldPos = highlighter.HighlightTilemap.CellToWorld(targetCell) + highlighter.HighlightTilemap.cellSize / 2f;

            Vector3 camPos2D = new Vector3(transform.position.x, transform.position.y, 0); // camera position
            Vector3 targetPos2D = new Vector3(targetWorldPos.x, targetWorldPos.y, 0); // target pos flattended with z = 0

            // only move camera if the mouse is outside the deadzone
            if (Vector3.Distance(camPos2D, targetPos2D) > deadzoneRadius)
            {
                // move smoove
                Vector3 desiredPosition = targetWorldPos + offset;
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * panSpeed);
            }
        }

    }

    public IEnumerator PanToLocation(Vector3 targetPos)
    {
        autoPanning = true; // disable mouse panning until panned to location
        Vector3 desiredPosition = targetPos + offset; // position -+ (0, 0, -10) (camera z pos)

        while (Vector3.Distance(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(desiredPosition.x, desiredPosition.y, 0)) > 0.05)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 6 * panSpeed); // lerp to target postion
            yield return null;
        }

        autoPanning = false; // restart mouse panning
        transform.position = desiredPosition; // snap to final location
    }

}
*/
/*
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class CameraPanner : MonoBehaviour
{
    [SerializeField] private float panSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float deadzoneRadius = 3.5f;
    [SerializeField] private Tilemap tilemap;

    private Camera cam;
    private ControlsManager highlighter;
    private bool autoPanning = false;

    private Vector2 minCameraPos;
    private Vector2 maxCameraPos;

    private void Start()
    {
        cam = GetComponent<Camera>();
        highlighter = ControlsManager.Instance;

        CalculateCameraBounds();
    }

    private void Update()
    {
        if (!autoPanning)
        {
            if (highlighter == null || !highlighter.enableFunction) return;

            Vector3Int targetCell = highlighter.LastHighlightedCell;
            Vector3 targetWorldPos = tilemap.CellToWorld(targetCell) + tilemap.cellSize / 2f;

            Vector3 camPos2D = new Vector3(transform.position.x, transform.position.y, 0);
            Vector3 targetPos2D = new Vector3(targetWorldPos.x, targetWorldPos.y, 0);

            if (Vector3.Distance(camPos2D, targetPos2D) > deadzoneRadius)
            {
                Vector3 desiredPosition = targetWorldPos + offset;
                desiredPosition = ClampCameraPosition(desiredPosition);
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * panSpeed);
            }
        }
    }

    public IEnumerator PanToLocation(Vector3 targetPos)
    {
        autoPanning = true;
        Vector3 desiredPosition = ClampCameraPosition(targetPos + offset);

        while (Vector3.Distance(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(desiredPosition.x, desiredPosition.y, 0)) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 6 * panSpeed);
            yield return null;
        }

        transform.position = desiredPosition;
        autoPanning = false;
    }

    private void CalculateCameraBounds()
    {
        Bounds bounds = tilemap.localBounds;

        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        minCameraPos = new Vector2(
            bounds.min.x + horzExtent,
            bounds.min.y + vertExtent
        );
        maxCameraPos = new Vector2(
            bounds.max.x - horzExtent,
            bounds.max.y - vertExtent
        );
    }

    private Vector3 ClampCameraPosition(Vector3 position)
    {
        float clampedX = Mathf.Clamp(position.x, minCameraPos.x, maxCameraPos.x);
        float clampedY = Mathf.Clamp(position.y, minCameraPos.y, maxCameraPos.y);
        return new Vector3(clampedX, clampedY, position.z);
    }
}
*/
