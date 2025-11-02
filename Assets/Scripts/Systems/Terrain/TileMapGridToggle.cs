using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapGridToggle : MonoBehaviour
{
    [Header("Material / Toggle")]
    public Material gridMaterial;
    public bool defaultEnabled = true;

    [Header("Grid Settings")]
    public Color gridColor = new Color(1f, 0.78f, 0f, 1f);
    public Vector2 cellSize = Vector2.one;
    public float thickness = 0.02f;
    [Range(0,1)] public float opacity = 1.0f;

    TilemapRenderer tilemapRenderer;
    Material runtimeMat;
    bool enabledState;

    void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        if (gridMaterial == null)
        {
            Shader s = Shader.Find("Unlit/TilemapGridOverlay");
            if (s != null)
                gridMaterial = new Material(s);
        }
    }

    void Start()
    {
        if (gridMaterial == null)
        {
            Debug.LogError("[TilemapGridToggle] Missing gridMaterial!");
            return;
        }

        runtimeMat = Instantiate(gridMaterial);
        ApplyPropertiesToMaterial();
        tilemapRenderer.material = runtimeMat;

        enabledState = defaultEnabled;
        runtimeMat.SetFloat("_GridEnabled", enabledState ? 1f : 0f);

        // Subscribe to ControlsManager input
        if (ControlsManager.Instance != null)
        {
            Debug.Log("Subbed to event");
            ControlsManager.Instance.OnToggleGrid += ToggleGrid;
        }
    }

    void OnDestroy()
    {
        if (ControlsManager.Instance != null)
        {
            ControlsManager.Instance.OnToggleGrid -= ToggleGrid;
        }
    }

    void Update()
    {
        ApplyPropertiesToMaterial(); // for live changes in inspector
    }

    void ApplyPropertiesToMaterial()
    {
        if (runtimeMat == null) return;
        runtimeMat.SetColor("_GridColor", gridColor);
        runtimeMat.SetVector("_CellSize", new Vector4(cellSize.x, cellSize.y, 0, 0));
        runtimeMat.SetFloat("_Thickness", thickness);
        runtimeMat.SetFloat("_Opacity", opacity);
    }

    public void ToggleGrid()
    {
        Debug.Log("balls");
        enabledState = !enabledState;
        if (runtimeMat != null)
            runtimeMat.SetFloat("_GridEnabled", enabledState ? 1f : 0f);
    }

    public void SetGridEnabled(bool on)
    {
        enabledState = on;
        if (runtimeMat != null)
            runtimeMat.SetFloat("_GridEnabled", enabledState ? 1f : 0f);
    }
}
