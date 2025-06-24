using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitSpawner : MonoBehaviour
{
    public static UnitSpawner Instance { get; private set; }
    public Tilemap highlightTilemap;
    public Transform unitFolder;

    private Vector3 positionOffset = new Vector3(0.5f, 0.5f, 0);

    [SerializeField] private GameObject unitPrefab; // assign in inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public Unit SpawnUnitFromTemplate(UnitData data, Vector3Int gridPos)
    {
        GameObject go = Instantiate(unitPrefab, unitFolder);
        Unit unit = go.GetComponent<Unit>();
        SpriteRenderer s = go.GetComponent<SpriteRenderer>();
        MovementRange m = go.GetComponent<MovementRange>();

        m.highlightTilemap = highlightTilemap;
        s.sprite = data.combatSprite;
        unit.unitClass = data.startingClass;
        unit.unitName = data.unitName;
        unit.unitTitle = data.unitTitle;
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

        unit.transform.position = GridManager.Instance.CellToWorld(gridPos)  - positionOffset;
        unit.GridPosition = (Vector2Int)gridPos; // is this even being used?

        return unit;
    }

    public Unit SpawnUnitFromSaveData(SavedUnitData data, Vector3Int gridPos)
    {
        GameObject go = Instantiate(unitPrefab, unitFolder);
        Unit unit = go.GetComponent<Unit>();

        unit.unitName = data.unitID;
        unit.unitClass = UnitClassDatabase.Instance.GetByID(data.unitClassName);

        unit.level = data.level;
        unit.maxHP = data.maxHP;
        unit.currentHP = data.currentHP;

        unit.strength = data.strength;
        unit.arcane = data.arcane;
        unit.defense = data.defense;
        unit.speed = data.speed;
        unit.skill = data.skill;
        unit.resistance = data.resistance;
        unit.luck = data.luck;

        unit.inventory.Clear();
        foreach (string id in data.inventoryIDs)
        {
            var item = ItemDatabase.Instance.GetByID(id);
            if (item != null) unit.AddItem(Instantiate(item));
        }

        if (!string.IsNullOrEmpty(data.equippedItemID))
        {
            var item = ItemDatabase.Instance.GetByID(data.equippedItemID);
            if (item != null) unit.Equip(item);
        }

        unit.transform.position = GridManager.Instance.CellToWorld(gridPos) - positionOffset;
        unit.GridPosition = (Vector2Int)gridPos;

        return unit;
    }
}
