using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance;
    public SaveGameDatabase saveGameDatabase; // i was being lazy, can move to SaveGameManager iyw

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        StartCoroutine(InitializeAllDatabases()); // i dont want to hear it
    } 

    // for true, ill fix it later
    public IEnumerator InitializeAllDatabases()
    {
        bool done = false;
        InitializeDatabase(() => done = true);

        yield return new WaitUntil(() => done);
    }

    public bool SaveGame (SaveGameData.SlotName slotName)
    {
        SaveGameData saveSlot = saveGameDatabase.GetBySlotName(slotName);

        if (saveSlot == null)
            return false;

        return saveSlot.SaveGame();
    }

    public bool LoadGame (SaveGameData.SlotName slotName)
    {
        SaveGameData saveSlot = saveGameDatabase.GetBySlotName(slotName);

         if (saveSlot == null)
        {
            Debug.Log($"{slotName} is null");
            return false;
        }

        return saveSlot.LoadGame();
    }

    // clunky but whatever
    public List<SaveGameData.SlotName> GetLoadableSaves ()
    {
        List<SaveGameData.SlotName> occupiedSlots = new List<SaveGameData.SlotName>();
        foreach (SaveGameData occupiedSlot in saveGameDatabase.GetByHasData(true))
        {
            occupiedSlots.Add(occupiedSlot.slotName);
        }
        return occupiedSlots;
        
    }
    public IReadOnlyList<SaveGameData.SlotName> GetOpenSaveSlots ()
    {
        List<SaveGameData.SlotName> emptySlots = new List<SaveGameData.SlotName>();
        foreach (SaveGameData emptySlot in saveGameDatabase.GetByHasData(false))
        {
            emptySlots.Add(emptySlot.slotName);
        }
        return emptySlots;
    }

    public void InitializeDatabase(Action onComplete)
    {
        int pending = 0;

        void Track(Action initCall)
        {
            pending++;
            initCall();
        }

        void OnOneDone()
        {
            pending--;
            if (pending <= 0)
            {
                onComplete?.Invoke();
            }
        }
        Track(() => saveGameDatabase.Initialize(OnOneDone));
    }
}