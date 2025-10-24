using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

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

    [Tooltip("How fast the camera follows the target (higher = snappier)")]
    public float panSmoothTime = 0.2f;

    [Tooltip("How fast the camera zooms (higher = faster zoom)")]
    public float zoomSmoothTime = 0.3f;

    private Vector3 currentVelocity = Vector3.zero;
    //private float zoomVelocity = 0f;

    private Camera cam;
    private Vector3 minCameraPos;
    private Vector3 maxCameraPos;

    private float halfCamHeight;
    private float halfCamWidth;

    // Cutscene variables
    public bool inCutscene = false; // to enable/disable to regular panner behaviour
    public Vector3 shakeOffset = Vector3.zero;
    void Start()
    {
        cam = Camera.main;

        // get cam dimensions
        halfCamHeight = cam.orthographicSize;
        halfCamWidth = halfCamHeight * cam.aspect;
        LoadGridBounds();
        ControlsManager.Instance.OnContextSwitch += HandleContextSwitch;
    }

    void HandleContextSwitch(InputContext newContext)
    {
        switch(newContext) 
        {
            case InputContext.Gameplay:
                inCutscene = false;
                break;
            case InputContext.Cutscene:
                inCutscene = true;
                break;
        }
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
            // transform.position = Vector3.Lerp(transform.position, targetWorldPos, cameraSpeed * Time.deltaTime);
            transform.position = Vector3.SmoothDamp(transform.position, targetWorldPos, ref currentVelocity, panSmoothTime);
        }
        transform.position += shakeOffset;
    }


    public IEnumerator PanToLocation(Vector3 targetPos, float speed)
    {
        Debug.Log("panning");
        Vector3 targetCameraPos = new Vector3(targetPos.x, targetPos.y, -10); // set the z to -10
        Vector3 velocity = Vector3.zero;

        while (Vector3.Distance(transform.position, targetCameraPos) > 0.075f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetCameraPos, ref velocity, 1f / speed);
            yield return null;
        }

        transform.position = targetCameraPos; // Snap to final position
    }

    public async UniTask PanToLocationAsync(Vector3 targetPos, float speed)
    {
        await PanToLocation(targetPos, speed).ToUniTask();
    }

    // Same as pan to location but just get location of unit
    public IEnumerator PanToUnit(string unitName, float speed)
    {
        Unit unit = UnitManager.Instance.FindUnitByName(unitName);
        if (unit)
        {
            yield return StartCoroutine(PanToLocation(unit.transform.position, speed));
        }
    }
    
    // gotta fix this, looks really bad atm
    public IEnumerator ShakeCamera(float intensity, float duration)
    {
        float elapsed = 0f;
        float percentComplete;
        float currentIntensity;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            percentComplete = elapsed / duration;
            currentIntensity = intensity * -0.7f * (Mathf.Pow(2f * percentComplete - 1f, 2f) + 1); // some basic parabola stuff, fades in an out shake, maybe add param controlling this
            float offsetX = UnityEngine.Random.Range(-0.5f, 0.5f) * currentIntensity;
            float offsetY = UnityEngine.Random.Range(-0.5f, 0.5f) * currentIntensity;

            shakeOffset = new Vector3(offsetX, offsetY, 0f);
            yield return null;
        }
        shakeOffset = Vector3.zero;
    }

    public void SetInCutscene(bool val)
    {
        inCutscene = val;
    }

    public IEnumerator ZoomCamera(float targetSize, float duration, float zoomSTime = -1)
    {
        Debug.Log("ZOoming");
        float startSize = cam.orthographicSize;
        float elapsed = 0f;
        float currentVelocity = 0f;
        if (zoomSTime == -1)
        {
            zoomSTime = zoomSmoothTime;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref currentVelocity, zoomSTime);
            yield return null;
        }

        cam.orthographicSize = targetSize;
    }

    public async UniTask ZoomCameraAsync(float targetSize, float duration, float zoomSTime = -1)
    {
        await ZoomCamera(targetSize, duration, zoomSTime).ToUniTask();
    }

    // Optionally, to instantly set zoom
    public void SetZoom(float targetSize)
    {
        cam.orthographicSize = targetSize;
    }
}

