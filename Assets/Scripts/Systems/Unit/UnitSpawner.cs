using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

// get highlight tilemap from grid manager
// get turn number from turn manager
public class UnitSpawner : MonoBehaviour
{
    public static UnitSpawner Instance { get; private set; }
    public Transform unitFolder;
    private Vector3 positionOffset = new Vector3(0.5f, 0.5f, 0);

    public List<UnitSpawnEvent> spawnEvents; 

    [System.Serializable]
    public class UnitSpawnEvent
    {
        public int spawnTurn;
        public GameObject unitToSpawn; // from a prefab per specific unit 
        public Vector2Int gridPos;
        public List<string> inventory_ids;

        [Header("OPTIONAL OVERWRITE DATA")]
        public UnitData unitData; // optional data
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
    private void Start()
    {
        TurnManager.Instance.OnTurnFlip += HandleTurnFlip;
    }

    public void HandleTurnFlip(int turnNum)
    {
        for (int i = spawnEvents.Count - 1; i >= 0; i--)
        {
            if (spawnEvents[i].spawnTurn == turnNum)
            {
                Debug.Log($"Spawning Unit at: {spawnEvents[i].gridPos}");
                Unit unit = SpawnUnitFromPrefab(spawnEvents[i].unitToSpawn, spawnEvents[i].gridPos, spawnEvents[i].unitData);
                SceneAnimationController.Instance.RegisterAnimator(unit.unitName, unit.animator);
                foreach(string itemid in spawnEvents[i].inventory_ids)
                {
                    unit.inventory.Add(new ItemInstance(GameManager.Instance.itemDatabase.GetByID(itemid)));
                }
                // equip first item in inventory
                foreach(ItemInstance item in unit.inventory.Items)
                {
                    if (item.IsWeapon)
                    {
                        unit.Equip(item);
                        break;
                    }
                }
                spawnEvents.RemoveAt(i);
            }
        }
    }

    public void SpawnRosterUnits(Vector3Int[] spawnPositions)
    {
        int index = 0;

        foreach (var entry in GameManager.Instance.PlayerRoster.Entries)
        {
            if (!entry.IsAlive || index >= spawnPositions.Length)
            {
                continue;
            }

            Vector3 worldPos = GridManager.Instance.CellToWorld(spawnPositions[index]) + new Vector3(0.5f, 0.5f, 0f);

            Unit unit = Object.Instantiate(
                entry.UnitPrefab,
                worldPos,
                Quaternion.identity,
                unitFolder
            );

            unit.ApplyRuntimeState(entry.RuntimeState);
            unit.GridPosition = (Vector2Int)spawnPositions[index];
            UnitManager.Instance.RegisterUnit(unit);
            SceneAnimationController.Instance.RegisterAnimator(unit.unitName, unit.animator); // since the units get spawned after Awake is called they dont get registered with the animation controller
            index++;
        }
    }
    
    public Unit SpawnUnitFromPrefab(GameObject unitPrefab, Vector2Int gridPos, UnitData data)
    {
        // base instantiate
        GameObject go = Instantiate(unitPrefab, unitFolder); // make game object under the unit folder
        Unit unit = go.GetComponent<Unit>(); // grab unit reference
        unit.GridPosition = gridPos; // set its grid pos 
        go.transform.position = GridManager.Instance.CellToWorld((Vector3Int)gridPos) + positionOffset; // set its real world pos
        MovementRange m = go.GetComponent<MovementRange>(); // grab movement range ref
        m.highlightTilemap = GridManager.Instance.highlightTilemap; // assign highlight map ref

        if (data != null) // if the optional data was provided, use it, if not, it will default to the prefabs data
        {
            unit.unitClass = data.startingClass;
            unit.unitName = data.unitName;
            unit.unitDescription = data.unitDescription;
            unit.team = data.team;

            unit.level = data.level;
            unit.maxHP = data.maxHP;
            unit.currentHP = data.maxHP;

            unit.strength = data.strength;
            unit.arcane = data.arcane;
            unit.defense = data.defense;
            unit.speed = data.speed;
            unit.skill = data.skill;
            unit.resistance = data.resistance;
            unit.luck = data.luck;
            
            foreach (var item in data.inventory.Items)
            {
                unit.inventory.Add(item); // instantiate if item has state
                // was i high when i wrote this
            }

            // need to init proficiencies b4 weapon equip otherwise itll error out and i don't want to write checkers
            if (data.proficiencyLevels == null)
            {
                WeaponProficiency prof = new WeaponProficiency();
                prof.Initialize();
                unit.proficiencyLevels = prof;
            }
            else
            {

                unit.proficiencyLevels = data.proficiencyLevels;
                unit.proficiencyLevels.Initialize();
            }

            unit.Equip(data.equippedItem);
        }

        UnitManager.Instance.RegisterUnit(unit); //register this cunt
        return unit;
        // perhaps a fade in effect?
    }

    // relics below that may be useful later depending on wat we do
    /*
    public Unit SpawnUnitFromTemplate(UnitData data, Vector3Int gridPos)
    {
        GameObject go = Instantiate(unitPrefab, unitFolder);
        Unit unit = go.GetComponent<Unit>();
        SpriteRenderer s = go.GetComponent<SpriteRenderer>();
        MovementRange m = go.GetComponent<MovementRange>();

        m.highlightTilemap = highlightTilemap;
        // changed infrastructure, this should be modified (noted in miro)
        if (data.animationPrefab) // add the animation prefab to the unit prefab if it (animPrefab) exists
        {
            GameObject animPrefab = Instantiate(data.animationPrefab, go.transform);
            unit.animPrefab = animPrefab;
            animPrefab.transform.localPosition = new Vector3(0, -0.5f, 0);
        }
        else
        {
            s.sprite = data.combatSprite;
        }
        unit.unitClass = data.startingClass;
        unit.unitName = data.unitName;
        unit.unitDescription = data.unitDescription;
        unit.team = data.team;

        unit.level = data.level;
        unit.maxHP = data.maxHP;
        unit.currentHP = data.maxHP;

        unit.strength = data.strength;
        unit.arcane = data.arcane;
        unit.defense = data.defense;
        unit.speed = data.speed;
        unit.skill = data.skill;
        unit.resistance = data.resistance;
        unit.luck = data.luck;

        unit.inventory.Clear();
        foreach (var item in data.startingInventory)
        {
            unit.AddItem(Instantiate(item)); // instantiate if item has state
            // was i high when i wrote this
        }

        // need to init proficiencies b4 weapon equip otherwise itll error out and i don't want to write checkers
        if (data.proficiencyLevels == null)
        {
            WeaponProficiency prof = new WeaponProficiency();
            prof.Initialize();
            unit.proficiencyLevels = prof;
        }
        else
        {

            unit.proficiencyLevels = data.proficiencyLevels;
            unit.proficiencyLevels.Initialize();
        }
        
        unit.Equip(data.equippedItem);

        if (data.animationPrefab)
        {
            unit.transform.position = GridManager.Instance.CellToWorld(gridPos) - positionOffset;
        }
        else
        {
            unit.transform.position = GridManager.Instance.CellToWorld(gridPos) - new Vector3(0.5f, 1f, 0);
        }
        unit.GridPosition = (Vector2Int)gridPos; // is this even being used?
        unit.combatSprite = data.combatSprite;
        return unit;
    }
    */
    // public Unit SpawnUnitFromSaveData(GameObject unitPrefab, SavedUnitData data, Vector3Int gridPos)
    // {
    //     GameObject go = Instantiate(unitPrefab, unitFolder);
    //     Unit unit = go.GetComponent<Unit>();

    //     unit.unitName = data.unitID;
    //     //unit.unitClass = UnitClassDatabase.Instance.GetByID(data.unitClassName);

    //     unit.level = data.level;
    //     unit.maxHP = data.maxHP;
    //     unit.currentHP = data.currentHP;

    //     unit.strength = data.strength;
    //     unit.arcane = data.arcane;
    //     unit.defense = data.defense;
    //     unit.speed = data.speed;
    //     unit.skill = data.skill;
    //     unit.resistance = data.resistance;
    //     unit.luck = data.luck;
    //     // foreach (string id in data.inventoryIDs)
    //     // {
    //     //     //var item = ItemDatabase.Instance.GetByID(id);
    //     //     if (item != null) unit.AddItem(Instantiate(item));
    //     // }

    //     // if (!string.IsNullOrEmpty(data.equippedItemID))
    //     // {
    //     //     //var item = ItemDatabase.Instance.GetByID(data.equippedItemID);
    //     //     if (item != null) unit.Equip(item);
    //     // }

    //     unit.transform.position = GridManager.Instance.CellToWorld(gridPos) - positionOffset;
    //     unit.GridPosition = (Vector2Int)gridPos;

    //     return unit;
    // }
}
