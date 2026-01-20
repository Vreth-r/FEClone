using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "Databases/Unit Addressable Database")]
public class UnitAddressableDatabase : ScriptableObject
{
    private readonly Dictionary<string, GameObject> cache = new();
    public bool IsInitialized { get; private set; }

    public void Initialize(Action onDBINITComplete)
    {
        if (IsInitialized)
        {
            onDBINITComplete?.Invoke();
            return;
        }

        Addressables.LoadAssetsAsync<GameObject>("UnitPrefab")
        .Completed += handle =>
        {
            IsInitialized = true;
            onDBINITComplete?.Invoke();
            var prefab = handle.Result;
            foreach (GameObject unit in prefab)
            {
                Unit script = unit.GetComponent<Unit>();
                if (script == null)
                {
                    Debug.LogError($"Unit prefab {unit} has no Unit component!");
                }
                else if (!cache.ContainsKey(script.unitID))
                {
                    cache.Add(script.unitID, unit);
                }
            }
        };
    }

    protected virtual void OnEnable()
    {
        IsInitialized = false;
        cache.Clear();
    }

    public GameObject GetPrefab(string unitID)
    {
        if (!IsInitialized)
        {
            Debug.LogError("UnitDatabase accessed before initialization!");
            return null;
        }

        cache.TryGetValue(unitID, out var prefab);
        return prefab;
    }

    public void DebugPrintThatShit()
    {
        Debug.Log($"{cache.Count} units registered.");
        foreach (var item in cache)
        {
            Debug.Log($"{item.Key}");
        }
    }
}
