using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraPanner : MonoBehaviour
{
    [SerializeField] private float panSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float deadzoneRadius = 3.5f; // radius where the mouse wont move

    private Camera cam;
    private MouseTileHighlighter highlighter;

    private void Start()
    {
        cam = GetComponent<Camera>();
        highlighter = MouseTileHighlighter.Instance;
    }

    private void Update()
    {
        if (highlighter == null || !highlighter.enableFunction) return;

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
