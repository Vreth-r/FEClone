using UnityEngine;
using System.Collections;

[System.Serializable]
public class CutsceneEvent
{
    public CutsceneEventType type;

    // Common params
    public Vector3 position;
    public float floatParam1;
    public float floatParam2;
    public float delay;

    // Optional: ID or string target for unit actions
    public string unitName;
}