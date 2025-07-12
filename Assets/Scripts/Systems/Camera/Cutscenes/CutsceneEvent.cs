using UnityEngine;

// Holds all the data for a given event
// Not all params are used in every event type
[System.Serializable]
public class CutsceneEvent
{
    public CutsceneEventType type; // cutscene type 
    public Vector3 vector3Param; // used for position
    public float floatParam1; // float params for speed and other numbers
    public float floatParam2;
    public string stringParam1; // string params for unit names and mini dialogue (maybe?) (like "!" or "?")
    public string stringParam2;
    public float delay; // pre event delay
}