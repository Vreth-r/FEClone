using UnityEngine;

public class CutsceneContext
{
    public CameraPanner cameraPanner;
    public UnitManager unitManager;
    public MonoBehaviour runner;
    // Add anything else needed to run cutscene events
    public CutsceneContext(CameraPanner cam, MonoBehaviour runner)
    {
        cameraPanner = cam;
        this.runner = runner;
    }
}