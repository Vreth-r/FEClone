using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIMenuBase
{
    public VisualElement Root { get; private set; }

    protected UIManagerTheSecond manager;

    public void Initialize(UIManagerTheSecond manager, VisualElement root)
    {
        this.manager = manager;
        Root = root;

        OnCreate();
    }

    // called when menu in instantiated
    protected virtual void OnCreate() { }

    // called when menu becomes top menu
    public virtual void OnOpen() { }

    // called when the menu is removed or covered
    public virtual void OnClose() { }

    public virtual void Tick(float deltaTime) { }
}

public interface IMenuWithData<in T>
{
    void SetData(T data);
}