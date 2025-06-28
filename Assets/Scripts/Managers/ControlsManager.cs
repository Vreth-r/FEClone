/*
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class ControlsManager : MonoBehaviour
{
    [Header("Tilemap Highlighting")]
    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private TileBase highlightTile;

    public static ControlsManager Instance { get; private set; }

    public bool EnableInput { get; set; } = true; // this would be for global use but for now ive replaced the old
    // mouse tile highlight flag calls with this.

    // Public read-only properties
    public Vector2Int moveInput;
    public bool SelectPressed { get; private set; }
    public bool CancelPressed { get; private set; }
    public Vector3Int LastHighlightedCell => lastCell;
    public Tilemap HighlightTilemap => highlightTilemap;

    private Vector3Int lastCell;
    private Vector3Int cursorCell;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        moveInput = new Vector2Int(0, 0);
        // Set initial cursor cell to world origin
        cursorCell = highlightTilemap.WorldToCell(Vector3.zero);
        lastCell = cursorCell;
        highlightTilemap.SetTile(cursorCell, highlightTile);
    }

    private void Update()
    {
        if (!EnableInput) return;

        ReadInput();

        if (moveInput != Vector2Int.zero)
        {
            MoveCursor(moveInput);
        }

        // Selection logic can be triggered here if needed
        if (SelectPressed)
        {
            Debug.Log("Select pressed on tile: " + lastCell);
        }

        if (CancelPressed)
        {
            Debug.Log("Cancel pressed");
        }
    }

    private void ReadInput()
    {
        // Reset each frame
        moveInput = Vector2Int.zero;
        SelectPressed = false;
        CancelPressed = false;

        // Movement
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            moveInput.y += 1;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            moveInput.y -= 1;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            moveInput.x -= 1;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            moveInput.x += 1;

        // Actions (extendable)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            SelectPressed = true;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
            CancelPressed = true;
    }

    private void MoveCursor(Vector2Int direction)
    {
        Vector3Int newCell = cursorCell + new Vector3Int(direction.x, direction.y, 0);

        // // Optional: Clamp newCell to tilemap bounds
        // BoundsInt bounds = highlightTilemap.cellBounds;
        // newCell.x = Mathf.Clamp(newCell.x, bounds.xMin, bounds.xMax - 1);
        // newCell.y = Mathf.Clamp(newCell.y, bounds.yMin, bounds.yMax - 1);

        // Update highlight
        if (newCell != lastCell)
        {
            highlightTilemap.SetTile(lastCell, null);
            highlightTilemap.SetTile(newCell, highlightTile);
            lastCell = newCell;
            cursorCell = newCell;
        }
    }
}
*/

// I dont fully understand this myself tbh
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputContext
{
    Gameplay,
    Menu
}

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance { get; private set; }

    private InputActions inputActions;
    public InputContext CurrentContext { get; private set; } = InputContext.Gameplay;

    public Vector2 MoveInput { get; private set; }
    public bool SelectPressed { get; private set; }

    public Vector2 NavigateInput { get; private set; }
    public bool SubmitPressed { get; private set; }
    public bool CancelPressed { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new InputActions();

        // Gameplay bindings
        inputActions.Gameplay.MoveCursor.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.MoveCursor.canceled += ctx => MoveInput = Vector2.zero;

        inputActions.Gameplay.Select.performed += ctx => SelectPressed = true;

        // UI bindings
        inputActions.Menu.Navigate.performed += ctx => NavigateInput = ctx.ReadValue<Vector2>();
        inputActions.Menu.Navigate.canceled += ctx => NavigateInput = Vector2.zero;

        inputActions.Menu.Submit.performed += ctx => SubmitPressed = true;
        inputActions.Menu.Cancel.performed += ctx => CancelPressed = true;
    }

    void OnEnable() => EnableCurrentMap();
    void OnDisable() => DisableAllMaps();

    void Update()
    {
        // reset one frame buttons
        SelectPressed = false;
        SubmitPressed = false;
        CancelPressed = false;
    }

    public void SetContext(InputContext context)
    {
        if (CurrentContext == context) return;

        DisableAllMaps();
        CurrentContext = context;
        EnableCurrentMap();
    }

    private void EnableCurrentMap()
    {
        if (CurrentContext == InputContext.Gameplay)
        {
            inputActions.Gameplay.Enable();
        }
        else if (CurrentContext == InputContext.Menu)
        {
            inputActions.Menu.Enable();
        }
    }

    private void DisableAllMaps()
    {
        inputActions.Gameplay.Disable();
        inputActions.Menu.Disable();
    }

}
