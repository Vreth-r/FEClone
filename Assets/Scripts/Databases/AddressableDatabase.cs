using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class AddressableDatabase<T> : ScriptableObject
    where T : ScriptableObject
{
    protected readonly List<T> items = new();
    public IReadOnlyList<T> Items => items;

    public bool IsInitialized { get; private set; }

    protected abstract string Label { get; }

    public void Initialize(Action onComplete = null)
    {
        if (IsInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        Debug.Log("Intialized Database");

        Addressables.LoadAssetsAsync<T>(
            Label,
            OnItemLoaded
        ).Completed += handle =>
        {
            IsInitialized = true;
            onComplete?.Invoke();
        };
    }

    protected virtual void OnEnable()
    {
        IsInitialized = false;
        items.Clear();
    }

    protected virtual void OnItemLoaded(T item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
        }
    }

    public T GetByID(string id)
    {
        if (typeof(IIdentifiable).IsAssignableFrom(typeof(T)))
        {
            foreach (var item in items)
            {
                if (((IIdentifiable)item).ID == id)
                    return item;
            }
        }

        Debug.LogError($"Item with ID '{id}' not found in {GetType().Name}");
        return null;
    }

    public void Release()
    {
        Addressables.Release(items);
        items.Clear();
        IsInitialized = false;
    }

#if UNITY_EDITOR
    public void Validate()
    {
        var seen = new HashSet<string>();

        foreach (var item in items)
        {
            if (!seen.Add(((IIdentifiable)item).ID))
                Debug.LogError($"Duplicate ID: {item.name}");
        }
    }
#endif
}
