using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseTileHihglighter : MonoBehaviour
{
    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private TileBase highlightTile;

    private Vector3Int lastCell;

    private void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int currentCell = highlightTilemap.WorldToCell(mouseWorldPos);

        /*
        if(!IsInsideMap(currentCell))
        {
            highlightTilemap.SetTile(lastCell, null);
            lastCell = Vector3Int.zero;
            return;
        } */

        if(currentCell == lastCell)
        {
            return;
        }

        // clear old
        highlightTilemap.SetTile(lastCell, null);

        // set new highlight
        highlightTilemap.SetTile(currentCell, highlightTile);
        lastCell = currentCell;
    }

/*
    bool IsInsideMap(Vector3Int cell)
    {
        return GridManager.Instance.IsInsideBounds(new Vector2Int(cell.x, cell.y));
    }
    */
}