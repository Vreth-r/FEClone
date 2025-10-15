using UnityEngine;
using UnityEngine.InputSystem;
using System;

public enum InputContext
{
    Gameplay,
    Menu
}

// ** IMPORTANT **
// this is hardcoded to make additions and removals more clear, will switch to dynamic later maybe if im not lazy
public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance { get; private set; }

    [Header("Context")]
    public InputContext CurrentContext { get; private set; } = InputContext.Gameplay;

    [Header("Gameplay Actions")]
    [SerializeField] private InputActionReference moveCursorAction;
    [SerializeField] private InputActionReference selectAction;
    [SerializeField] private InputActionReference toggleGridAction;

    [Header("Menu Actions")]
    [SerializeField] private InputActionReference navigateAction;
    [SerializeField] private InputActionReference submitAction;
    [SerializeField] private InputActionReference cancelAction;

    // Exposed input values
    public Vector2 MoveInput { get; private set; }
    public Vector2 NavigateInput { get; private set; }

    // EVENTS
    public event Action OnSubmit;
    public event Action OnSelect;
    public event Action OnCancel;

    public event Action OnToggleGrid;

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        // --- Gameplay bindings that tie to events ---
        // i might swap this to per bind references in each object, but this works for things that arent monos by subbing to events (i think)
        if (moveCursorAction != null)
        {
            moveCursorAction.action.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            moveCursorAction.action.canceled += ctx => MoveInput = Vector2.zero;
        }

        if (selectAction != null)
        {
            selectAction.action.performed += ctx => OnSelect?.Invoke();
        }

        if (toggleGridAction != null)
        {
            toggleGridAction.action.performed += ctx => OnToggleGrid?.Invoke();
        }

        // --- Menu bindings ---
        if (navigateAction != null)
        {
            navigateAction.action.performed += ctx => NavigateInput = ctx.ReadValue<Vector2>();
            navigateAction.action.canceled += ctx => NavigateInput = Vector2.zero;
        }

        if (submitAction != null)
        {
            submitAction.action.performed += ctx => OnSubmit?.Invoke();
        }

        if (cancelAction != null)
        {
            cancelAction.action.performed += ctx => OnCancel?.Invoke();
        }
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
        {
            moveCursorAction?.action.Enable();
            selectAction?.action.Enable();
            toggleGridAction?.action.Enable();
        }
        else if (CurrentContext == InputContext.Menu)
        {
            navigateAction?.action.Enable();
            submitAction?.action.Enable();
            cancelAction?.action.Enable();
        }
    }

    private void DisableAllMaps()
    {
        moveCursorAction?.action.Disable();
        selectAction?.action.Disable();
        toggleGridAction?.action.Disable();
        navigateAction?.action.Disable();
        submitAction?.action.Disable();
        cancelAction?.action.Disable();
    }
}
