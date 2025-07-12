using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cutscene Manager! 
public class CutsceneManager : MonoBehaviour
{
    public CameraPanner cameraPanner; // camera panner ref to do things with
    public CutsceneDescriptionData testCutscene; // Testing only

    void Start() // testing only
    {
        StartCoroutine(TestRun());
    }

    public IEnumerator TestRun() // testing only
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(PlayCutscene(testCutscene));
    }

    // play entire cutscene
    public IEnumerator PlayCutscene(CutsceneDescriptionData data)
    {
        cameraPanner.SetInCutscene(true); // stop camera panner from doing regular behaviour 
        // itterate through all steps in cutscene description
        foreach (var step in data.steps)
        {
            List<Coroutine> eventCoroutines = new List<Coroutine>(); // to run coroutines in parallel 
            foreach (var e in step.events) // add all events in steps to coroutines list and start them
            {
                eventCoroutines.Add(StartCoroutine(RunEvent(e)));
            }

            foreach (var c in eventCoroutines) // Wait for all in this step to complete
            {
                yield return c;
            }
        }
        cameraPanner.SetInCutscene(false); // restart regular behaviour 
    }

    // coroutine to run a cutscene event
    private IEnumerator RunEvent(CutsceneEvent e)
    {
        yield return new WaitForSeconds(e.delay); // generic delay

        // call different coroutines based on event type
        switch (e.type)
        {
            case CutsceneEventType.PanToLocation:
                yield return cameraPanner.PanToLocation(e.vector3Param, e.floatParam1);
                break;
            case CutsceneEventType.PanToUnit:
                yield return cameraPanner.PanToUnit(e.stringParam1, e.floatParam1);
                break;
            case CutsceneEventType.CameraShake:
                yield return cameraPanner.ShakeCamera(e.floatParam1, e.floatParam2);
                break;
            case CutsceneEventType.UnitJump:
                yield return UnitManager.Instance.JumpUnit(e.stringParam1, e.floatParam1);
                break;
            case CutsceneEventType.Wait:
                yield return new WaitForSeconds(e.floatParam1);
                break;
        }
    }
}
