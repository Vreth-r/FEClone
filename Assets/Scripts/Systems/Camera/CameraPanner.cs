using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraPanner : MonoBehaviour
{
    [SerializeField] private float panSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); // this shouldnt matter but just in case

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

        // move smoove
        Vector3 desiredPosition = targetWorldPos + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * panSpeed);
    }
}