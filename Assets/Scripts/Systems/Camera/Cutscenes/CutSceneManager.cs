using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public CameraPanner cameraPanner;
    public UnitManager unitManager;
    public CutsceneDescriptionData testCutscene;

    public void Start()
    {
        PlayCutscene(testCutscene);
    }

    public IEnumerator PlayCutscene(CutsceneDescriptionData data)
    {
        foreach (var step in data.steps)
        {
            List<Coroutine> coroutines = new List<Coroutine>();

            foreach (var e in step.events)
            {
                coroutines.Add(StartCoroutine(RunEvent(e)));
            }

            foreach (var c in coroutines)
            {
                yield return c; // Wait for all in this step to complete
            }
        }
    }

    private IEnumerator RunEvent(CutsceneEvent e)
    {
        yield return new WaitForSeconds(e.delay);

        switch (e.type)
        {
            case CutsceneEventType.PanToLocation:
                yield return cameraPanner.PanToLocation(e.position, e.floatParam1);
                break;
            case CutsceneEventType.CameraShake:
                yield return cameraPanner.ShakeCamera(e.floatParam1, e.floatParam2);
                break;
            /*
            case CutsceneEventType.UnitJump:
                yield return unitManager.JumpUnit(e.targetUnit, e.position);
                break;
            */
        }
    }
}
public enum CutsceneEventType
{
    PanToLocation,
    CameraShake,
    UnitJump,
    // Add more types here
}