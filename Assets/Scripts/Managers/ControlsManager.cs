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

    private bool selectPressed;
    public bool SelectPressed => Consume(ref selectPressed);

    private bool submitPressed;
    public bool SubmitPressed => Consume(ref submitPressed);

    private bool cancelPressed;
    public bool CancelPressed => Consume(ref cancelPressed);

    private Vector2 navigateInput;
    public Vector2 NavigateInput => navigateInput;

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

        // Bind gameplay
        inputActions.Gameplay.MoveCursor.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.MoveCursor.canceled += ctx => MoveInput = Vector2.zero;

        inputActions.Gameplay.Select.performed += ctx => selectPressed = true;

        // Bind Menu
        inputActions.Menu.Navigate.performed += ctx => navigateInput = ctx.ReadValue<Vector2>();
        inputActions.Menu.Navigate.canceled += ctx => navigateInput = Vector2.zero;

        inputActions.Menu.Submit.performed += ctx => submitPressed = true;
        inputActions.Menu.Cancel.performed += ctx => cancelPressed = true;
    }

    void OnEnable() => EnableCurrentMap();
    void OnDisable() => DisableAllMaps();

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
            inputActions.Gameplay.Enable();
        else if (CurrentContext == InputContext.Menu)
            inputActions.Menu.Enable();
    }

    private void DisableAllMaps()
    {
        inputActions.Gameplay.Disable();
        inputActions.Menu.Disable();
    }

    // Utility to consume a bool flag for one-time reads
    private bool Consume(ref bool inputFlag)
    {
        if (!inputFlag) return false;
        inputFlag = false;
        return true;
    }
}
