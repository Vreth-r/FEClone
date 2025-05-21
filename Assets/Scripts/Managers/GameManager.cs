using UnityEngine;
using System.Collections.Generic;

// Ultra persistent script for "global" variable tracking

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // currency
    public int Gold { get; private set; }

    // Convoy Inventory (upgrading to its own class later)
    public List<Item> convoy = new();

    // recruited unit ID (might upgrade to scriptables later)
    public List<string> recruitedUnitIDs = new();

    // Optional: Global flags
    public HashSet<string> globalFlags = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Gold Management
    public void AddGold(int amount) => Gold += amount;
    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            return true;
        }
        return false;
    }

    // Convoy Management
    public void AddToConvoy(Item item) => convoy.Add(item);
    public void RemoveFromConvoy(Item item) => convoy.Remove(item);
    public bool ConvoyContains(Item item) => convoy.Contains(item);

    // Unit Management
    public void RecruitUnit(string unitID)
    {
        if (!recruitedUnitIDs.Contains(unitID)) recruitedUnitIDs.Add(unitID);
    }

    public bool IsUnitRecruited(string unitID) => recruitedUnitIDs.Contains(unitID);
}