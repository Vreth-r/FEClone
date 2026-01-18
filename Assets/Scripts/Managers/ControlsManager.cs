using UnityEngine;
using UnityEngine.InputSystem;
using System;

public enum InputContext
{
    Gameplay,
    Menu,
    Cutscene
}

// ** IMPORTANT **
// this is hardcoded to make additions and removals more clear, will switch to dynamic later maybe if im not lazy and theres a need

// while this is hardcoded you must update this with every new bind and map added. also update CutsceneYarnCommands.SetControlContext() if adding a new context
public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance { get; private set; }

    [Header("Context")]
    public InputContext CurrentContext { get; private set; } = InputContext.Menu;

    [Header("Gameplay Actions")]
    [SerializeField] private InputActionReference moveCursorAction;
    [SerializeField] private InputActionReference selectAction;
    [SerializeField] private InputActionReference toggleGridAction;
    [SerializeField] private InputActionReference pauseGameAction;
    [SerializeField] private InputActionReference interactAction; // for camp stuff

    [Header("Menu Actions")]
    [SerializeField] private InputActionReference navigateAction;
    [SerializeField] private InputActionReference submitAction;
    [SerializeField] private InputActionReference cancelAction;

    /* unsure if these need to be here cause yarn
    [Header("Cutscene Actions")]
    [SerializeField] private InputActionReference hurryAction;
    [SerializeField] private InputActionReference nextLineAction;
    */

    // Exposed input values
    public Vector2 MoveInput { get; private set; }
    public Vector2 NavigateInput { get; private set; }

    // EVENTS
    public event Action OnSubmit;
    public event Action OnSelect;
    public event Action OnCancel;
    public event Action OnPause;
    public event Action OnInteract;

    public event Action OnToggleGrid;

    public event Action<InputContext> OnContextSwitch;

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

        if (pauseGameAction != null)
        {
            pauseGameAction.action.performed += ctx => OnPause?.Invoke();
        }

        if (interactAction != null)
        {
            interactAction.action.performed += ctx => OnInteract?.Invoke();
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

        // notifiy subscribers
        OnContextSwitch?.Invoke(CurrentContext);
    }

    private void EnableCurrentMap()
    {
        // yarg there exists an InputContext.Cutscene that just has the yarn binds but they dont need to be here
        // cause yarn handles its own binds but having the map is useful to lock out controls during cutscenes
        if (CurrentContext == InputContext.Gameplay)
        {
            moveCursorAction?.action.Enable();
            selectAction?.action.Enable();
            toggleGridAction?.action.Enable();
            pauseGameAction?.action.Enable();
            interactAction?.action.Enable();
        }
        else if (CurrentContext == InputContext.Menu)
        {
            navigateAction?.action.Enable();
            submitAction?.action.Enable();
            cancelAction?.action.Enable();
        }
        else if (CurrentContext == InputContext.Cutscene)
        {
            return;
        }
    }

    private void DisableAllMaps()
    {
        moveCursorAction?.action.Disable();
        selectAction?.action.Disable();
        toggleGridAction?.action.Disable();
        pauseGameAction?.action.Disable();
        interactAction?.action.Disable();
        navigateAction?.action.Disable();
        submitAction?.action.Disable();
        cancelAction?.action.Disable();
    }
}
