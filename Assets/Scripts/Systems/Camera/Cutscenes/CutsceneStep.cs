using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutsceneStep
{
    public List<CutsceneEvent> events; // All run in parallel
}