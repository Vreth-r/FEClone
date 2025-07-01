using UnityEngine;
using UnityEngine.InputSystem;
using System;

public enum InputContext
{
    Gameplay,
    Menu
}

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance { get; private set; }

    public InputContext CurrentContext { get; private set; } = InputContext.Gameplay;

    private InputActions inputActions;

    // Exposed movement and navigation vectors
    public Vector2 MoveInput { get; private set; }
    public Vector2 NavigateInput { get; private set; }

    // EVENTS
    public event Action OnSubmit;
    public event Action OnSelect;
    public event Action OnCancel;

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

        inputActions.Gameplay.Select.performed += ctx => OnSelect?.Invoke();

        // UI bindings
        inputActions.Menu.Navigate.performed += ctx => NavigateInput = ctx.ReadValue<Vector2>();
        inputActions.Menu.Navigate.canceled += ctx => NavigateInput = Vector2.zero;

        inputActions.Menu.Submit.performed += ctx => OnSubmit?.Invoke();
        inputActions.Menu.Cancel.performed += ctx => OnCancel?.Invoke();
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
}
