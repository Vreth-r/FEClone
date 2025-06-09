using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseTileHighlighter : MonoBehaviour
{
    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private TileBase highlightTile;
    public bool enableFunction = true;
    public static MouseTileHighlighter Instance;

    private Vector3Int lastCell;
    private Vector3 lastMousePosition;
    private Vector3Int keyboardCursor;
    private bool usingKeyboard = false;
    public Vector3Int LastHighlightedCell => lastCell;
    public Tilemap HighlightTilemap => highlightTilemap;

    private void Awake()
    {
        Instance = this; // assign singleton
        lastMousePosition = Input.mousePosition;
        keyboardCursor = Vector3Int.zero;
    }

    private void Update()
    {
        if(!enableFunction)
        {
            highlightTilemap.SetTile(lastCell, null);
            return;
        }

        usingKeyboard = false;
        Vector3Int currentCell = lastCell;

        Vector3Int direction = Vector3Int.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            direction.y += 1;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            direction.y -= 1;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            direction.x -= 1;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            direction.x += 1;

        if (direction != Vector3Int.zero)
        {
            usingKeyboard = true;
            keyboardCursor += direction;
            currentCell = keyboardCursor;
        }

        if (!usingKeyboard)
        {
            // only move if the mouse has moved
            if (Input.mousePosition == lastMousePosition) return;

            lastMousePosition = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentCell = highlightTilemap.WorldToCell(mouseWorldPos);
            keyboardCursor = currentCell; // sync em
        }

        if (currentCell == lastCell) return;

        // clear old
        highlightTilemap.SetTile(lastCell, null);

        // set new highlight
        highlightTilemap.SetTile(currentCell, highlightTile);
        lastCell = currentCell;
    }
}