using UnityEngine;

[System.Serializable]
public class ParticleEffectStep
{
    [Tooltip("Prefab with one or more ParticleSystems.")]
    public GameObject effectPrefab;

    [Header("Timing")]
    public float delay = 0f;
    public float lifetime = 1.5f;

    [Header("Specifics")]
    public string destination;

    [Header("Travel Settings")]
    [Tooltip("If true, this effect travels from origin to target.")]
    public bool isTravelEffect = false;
    public float speed = 5f;

    [Header("Parallel Grouping")]
    [Tooltip("Steps with the same groupID run together.")]
    public int groupID = 0;
}
