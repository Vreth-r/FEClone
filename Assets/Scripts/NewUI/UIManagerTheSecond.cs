using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public class UIManagerTheSecond : MonoBehaviour
{
    public static UIManagerTheSecond Instance { get; private set; }

    [Header("UI Toolkit")]
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private List<MenuDefinition> menus;

    private readonly Stack<UIMenuBase> menuStack = new();
    private readonly Dictionary<string, MenuDefinition> menuLookup = new();

    private VisualElement root;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        root = uiDocument.rootVisualElement;

        foreach (var menu in menus)
        {
            menuLookup[menu.id] = menu;
        }
    }

    // event bindings for menus
    private void Start()
    {
        // Subscribe to ControlsManager input
        if (ControlsManager.Instance != null)
        {
            ControlsManager.Instance.OnPause += HandlePause;
            ControlsManager.Instance.OnCancel += () => {CloseTopMenu(); };
        }
    }

    private void HandlePause()
    {
        if(menuStack.Count != 0) return;
        OpenMenu<PauseMenuTheSecond>("PauseMenu");
    }

    // Open Menu (no data)
    public void OpenMenu<T>(string id) where T : UIMenuBase, new()
    {
        OpenMenuInternal<T>(id, null);
    }

    // Open Menu (with data)
    public void OpenMenu<T, TData>(string id, TData data)
        where T : UIMenuBase, new()
    {
        OpenMenuInternal<T>(id, data);
    }

    private void OpenMenuInternal<T>(string id, object data)
        where T : UIMenuBase, new()
    {
        if (!menuLookup.TryGetValue(id, out var definition))
        {
            Debug.LogError($"Menu '{id}' not found");
            return;
        }

        if (definition.menuType == UIMenuType.Screen)
        {
            ClearStack();
        }

        var menuRoot = definition.uxml.Instantiate();
        if (definition.stylesheet != null)
        {
            menuRoot.styleSheets.Add(definition.stylesheet);
        }

        root.Add(menuRoot);

        var menu = new T();
        menu.Initialize(this, menuRoot);

        if (data != null && menu is IMenuWithData<object> == false)
        {
            TrySetData(menu, data);
        }

        menu.OnOpen();
        menuStack.Push(menu);

        // might need to make a cond where this doesnt proc on popup menus
        ControlsManager.Instance.SetContext(InputContext.Menu);
    }

    private void TrySetData(UIMenuBase menu, object data)
    {
        var interfaces = menu.GetType().GetInterfaces();
        foreach (var i in interfaces)
        {
            if (i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IMenuWithData<>))
            {
                i.GetMethod("SetData")?.Invoke(menu, new[] { data });
                return;
            }
        }
    }

    // close top menu
    public void CloseTopMenu()
    {
        if (menuStack.Count == 0)
        {
            return;
        }

        var menu = menuStack.Pop();
        menu.OnClose();
        menu.Root.RemoveFromHierarchy();
        if(menuStack.Count == 0)
        {
            ControlsManager.Instance.SetContext(InputContext.Gameplay);
        }
    }

    // clear all menus
    public void ClearStack()
    {
        while (menuStack.Count > 0)
        {
            CloseTopMenu();
        }
    }
}