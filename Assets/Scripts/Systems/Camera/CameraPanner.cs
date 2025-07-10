using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

/* READ THIS
so the camera follows the cursor pretty closely in the ds titles of fe, so i might just copy that
when selecting a character, you move them with the cursor, and when you click move, the camera goes to the 
center point between the starting position and the end position while the character moves, it then centers on the character
and the action menu pops up
*/

public class CameraPanner : MonoBehaviour
{
    [Header("References")]
    public Grid grid;
    public Tilemap tilemap;

    [Header("Cursor")]
    public CursorController cursor;

    [Header("Camera Settings")]
    public float cameraSpeed = 1f;

    private Camera cam;
    private Vector3 minCameraPos;
    private Vector3 maxCameraPos;

    private float halfCamHeight;
    private float halfCamWidth;

    // Cutscene variables
    private bool inCutscene = false;
    public Vector3 shakeOffset = Vector3.zero;
    private float shakeTime = 0f;
    void Start()
    {
        cam = Camera.main;

        // get cam dimensions
        halfCamHeight = cam.orthographicSize;
        halfCamWidth = halfCamHeight * cam.aspect;
        Coroutine testCoroutine;
        testCoroutine = StartCoroutine(ShakeCamera(0.1f, 1f));
    }

    public void LoadGridBounds()
    {
        Bounds mapBounds = tilemap.localBounds;
        // clamp
        minCameraPos = mapBounds.min + new Vector3(halfCamWidth, halfCamHeight, 0f);
        maxCameraPos = mapBounds.max - new Vector3(halfCamWidth, halfCamHeight, 0f);
    }

    void LateUpdate()
    {
        if (!inCutscene)
        {
            // convert grid cursor position to world space
            Vector3 targetWorldPos = grid.CellToWorld(cursor.GetCursorGridPosition()) + grid.cellSize / 2f;

            // maintain og z 
            targetWorldPos.z = transform.position.z;

            // clamp the targetpos within cam bounds
            targetWorldPos.x = Mathf.Clamp(targetWorldPos.x, minCameraPos.x, maxCameraPos.x);
            targetWorldPos.y = Mathf.Clamp(targetWorldPos.y, minCameraPos.y, maxCameraPos.y);

            // smoove cam movement
            transform.position = Vector3.Lerp(transform.position, targetWorldPos, cameraSpeed * Time.deltaTime);
        }
        transform.position += shakeOffset;
    }
    

    public IEnumerator PanToLocation(Vector3 targetGridPos, float speed)
    {
        Vector3Int targetGridPosInt = new Vector3Int(
            Mathf.RoundToInt(targetGridPos.x),
            Mathf.RoundToInt(targetGridPos.y),
            -10);

        inCutscene = true;

        // Optional: Convert to world space if needed
        // Vector3 targetWorldPos = highlighter.HighlightTilemap.CellToWorld(targetGridPosInt) + highlighter.HighlightTilemap.cellSize / 2f;

        Vector3 targetWorldPos = targetGridPos; // already world pos?
        Vector3 desiredPosition = targetWorldPos; //+ offset;

        while (Vector3.Distance(transform.position, desiredPosition) > 0.075f)
        {
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = desiredPosition; // Snap to final position
        inCutscene = false;
    }

/*
    public IEnumerator PanToUnit(string unitName, float speed)
    {
        Vector3Int targetGridPosInt = new Vector3Int(
            Mathf.RoundToInt(targetGridPos.x),
            Mathf.RoundToInt(targetGridPos.y),
            -10);

        inCutscene = true;

        // Optional: Convert to world space if needed
        // Vector3 targetWorldPos = highlighter.HighlightTilemap.CellToWorld(targetGridPosInt) + highlighter.HighlightTilemap.cellSize / 2f;

        Vector3 targetWorldPos = targetGridPos; // already world pos?
        Vector3 desiredPosition = targetWorldPos; //+ offset;

        while (Vector3.Distance(transform.position, desiredPosition) > 0.075f)
        {
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = desiredPosition; // Snap to final position
        inCutscene = false;
    }
*/
    public IEnumerator ShakeCamera(float intensity, float duration)
    {
        inCutscene = true;
        float elapsed = 0f;
        float seedX = Random.Range(0f, 1000f);
        float seedY = Random.Range(0f, 1000f);
        float percentComplete;
        float currentIntensity;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            shakeTime += Time.deltaTime * 5f;// * frequency;
            percentComplete = elapsed / duration;
            currentIntensity = intensity * -0.7f * (Mathf.Pow(2f * percentComplete - 1f, 2f) + 1); // some basic parabola stuff
            float offsetX = Random.Range(-0.5f, 0.5f) * currentIntensity;
            float offsetY = Random.Range(-0.5f, 0.5f) * currentIntensity;


            shakeOffset = new Vector3(offsetX, offsetY, 0f);
            yield return null;
        }
        shakeOffset = Vector3.zero;
        inCutscene = false;
    }
}

