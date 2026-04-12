using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

[CreateAssetMenu(menuName = "Databases/Unit Addressable Database")]
public class UnitAddressableDatabase : ScriptableObject
{
    private readonly Dictionary<string, GameObject> cache = new();

    public bool IsInitialized { get; private set; }
    public bool IsInitializing { get; private set; }

    private const string LABEL = "UnitPrefab";

    public void Initialize(Action onComplete = null)
    {
        if (IsInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        if (IsInitializing)
        {
            Debug.LogWarning("[UnitDB] Already initializing.");
            return;
        }

        IsInitializing = true;
        cache.Clear();

        Debug.Log($"[UnitDB] Loading label: {LABEL}");

        Addressables.LoadResourceLocationsAsync(LABEL).Completed += locHandle =>
        {
            if (locHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[UnitDB] Failed to get locations for '{LABEL}'");
                Finish(onComplete);
                return;
            }

            var locations = locHandle.Result;

            if (locations == null || locations.Count == 0)
            {
                Debug.LogError($"[UnitDB] No locations found for '{LABEL}'");
                Finish(onComplete);
                return;
            }

            int total = locations.Count;
            int completed = 0;

            Debug.Log($"[UnitDB] Found {total} unit prefabs");

            foreach (var loc in locations)
            {
                Addressables.LoadAssetAsync<GameObject>(loc).Completed += handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        var prefab = handle.Result;

                        if (prefab == null)
                        {
                            Debug.LogWarning($"[UnitDB] Null prefab at {loc.PrimaryKey}");
                        }
                        else
                        {
                            Unit unit = prefab.GetComponent<Unit>();

                            if (unit == null)
                            {
                                Debug.LogError($"[UnitDB] {prefab.name} has no Unit component!");
                            }
                            else if (!cache.ContainsKey(unit.unitID))
                            {
                                cache.Add(unit.unitID, prefab);
                            }
                            else
                            {
                                Debug.LogWarning($"[UnitDB] Duplicate unitID: {unit.unitID}");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[UnitDB] Failed to load: {loc.PrimaryKey}");
                    }

                    completed++;

                    if (completed >= total)
                    {
                        Debug.Log($"[UnitDB] Finished loading ({cache.Count}/{total} valid)");
                        Finish(onComplete);
                    }
                };
            }
        };
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
        cache.Clear();
    }

    public GameObject GetPrefab(string unitID)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[UnitDB] Accessed before initialization!");
            return null;
        }

        if (string.IsNullOrEmpty(unitID))
        {
            Debug.LogError("[UnitDB] Invalid unitID (null or empty)");
            return null;
        }

        if (cache.TryGetValue(unitID, out var prefab))
            return prefab;

        Debug.LogError($"[UnitDB] Unit '{unitID}' not found!");
        return null;
    }

    public void DebugPrintThatShit()
    {
        Debug.Log($"[UnitDB] {cache.Count} units registered:");
        foreach (var kvp in cache)
        {
            Debug.Log($" - {kvp.Key}");
        }
    }
}