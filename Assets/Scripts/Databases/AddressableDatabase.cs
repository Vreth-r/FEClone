using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public abstract class AddressableDatabase<T> : ScriptableObject
    where T : ScriptableObject
{
    protected readonly List<T> items = new();
    public IReadOnlyList<T> Items => items;
    private Dictionary<string, T> idLookup = new();

    public bool IsInitialized { get; private set; }
    public bool IsInitializing { get; private set; }

    protected abstract string Label { get; }

    public void Initialize(Action onComplete = null)
    {
        if (IsInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        if (IsInitializing)
        {
            Debug.LogWarning($"[{GetType().Name}] Already initializing.");
            return;
        }

        IsInitializing = true;
        items.Clear();
        idLookup.Clear(); // IMPORTANT if you're using the dictionary version

        Debug.Log($"[Addressables] Loading label: {Label}");

        Addressables.LoadResourceLocationsAsync(Label).Completed += locHandle =>
        {
            if (locHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[Addressables] Failed to get locations for '{Label}'");
                Finish(onComplete);
                return;
            }

            IList<IResourceLocation> locations = locHandle.Result;

            Debug.Log($"[Addressables] Label '{Label}' has {locations.Count} locations");

            foreach (var loc in locations)
            {
                Debug.Log($" - {loc.PrimaryKey}");
            }

            if (locations == null || locations.Count == 0)
            {
                Debug.LogError($"[Addressables] No locations found for '{Label}'");
                Finish(onComplete);
                return;
            }

            int total = locations.Count;
            int completed = 0;

            Debug.Log($"[Addressables] Found {total} assets for '{Label}'");

            foreach (var loc in locations)
            {
                Addressables.LoadAssetAsync<T>(loc).Completed += handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        if (handle.Result != null)
                        {
                            OnItemLoaded(handle.Result); // ✅ USE THIS INSTEAD
                        }
                        else
                        {
                            Debug.LogWarning($"[Addressables] Null asset at {loc.PrimaryKey}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[Addressables] Failed to load: {loc.PrimaryKey}");
                    }

                    completed++;

                    if (completed >= total)
                    {
                        Debug.Log($"[Addressables] Finished loading '{Label}' ({items.Count}/{total} succeeded)");
                        Finish(onComplete);
                    }
                };
            }
        };
    }

    protected void OnItemLoaded(T item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);

            if (item is IIdentifiable identifiable)
            {
                if (!idLookup.ContainsKey(identifiable.ID))
                {
                    idLookup.Add(identifiable.ID, item);
                }
                else
                {
                    Debug.LogError($"[{GetType().Name}] Duplicate ID detected: {identifiable.ID}");
                }
            }
            else
            {
                Debug.LogError($"[{GetType().Name}] Item {item.name} does not implement IIdentifiable");
            }
        }
    }

    private void Finish(Action onComplete)
    {
        IsInitialized = true;
        IsInitializing = false;
        onComplete?.Invoke();
    }

    protected virtual void OnEnable()
    {
        IsInitialized = false;
        IsInitializing = false;
        items.Clear();
        idLookup.Clear();
    }

    public void Release()
    {
        foreach (var item in items)
        {
            Addressables.Release(item);
        }

        items.Clear();
        IsInitialized = false;
    }

    public T GetByID(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError($"[{GetType().Name}] GetByID called with null or empty ID");
            return null;
        }

        if (idLookup.TryGetValue(id, out var item))
        {
            return item;
        }

        Debug.LogError($"[{GetType().Name}] Item with ID '{id}' not found.");
        return null;
    }
}