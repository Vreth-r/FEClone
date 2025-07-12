using System.Collections.Generic;

// a single step in a cutscene, can have multiple events that run in parallel
[System.Serializable]
public class CutsceneStep
{
    public List<CutsceneEvent> events;
}