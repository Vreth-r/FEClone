using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class CutsceneYarnCommands : MonoBehaviour
{
    // Example: <<PanToLocation 0 10 0 2>>
    [YarnCommand("PanToLocation")]
    public static YarnTask PanToLocation(float x, float y, float z, float speed)
    {
        return CutsceneManager.Instance != null
            ? CutsceneManager.Instance.YarnCoroutine(CutsceneManager.Instance.cameraPanner.PanToLocation(new Vector3(x, y, z), speed))
            : YarnTask.CompletedTask;
    }

    // Example: <<PanToUnit "Ylru" 2>>
    [YarnCommand("PanToUnit")]
    public static YarnTask PanToUnit(string unitName, float speed)
    {
        return CutsceneManager.Instance != null
            ? CutsceneManager.Instance.YarnCoroutine(CutsceneManager.Instance.cameraPanner.PanToUnit(unitName, speed))
            : YarnTask.CompletedTask;
    }

    // Example: <<CameraShake 0.5 0.2>>
    [YarnCommand("CameraShake")]
    public static YarnTask CameraShake(float intensity, float duration)
    {
        return CutsceneManager.Instance != null
            ? CutsceneManager.Instance.YarnCoroutine(CutsceneManager.Instance.cameraPanner.ShakeCamera(intensity, duration))
            : YarnTask.CompletedTask;
    }

    // Example: <<UnitJump "Ylru" 1.2>>
    [YarnCommand("UnitJump")]
    public static YarnTask UnitJump(string unitName, float power)
    {
        return CutsceneManager.Instance != null
            ? CutsceneManager.Instance.YarnCoroutine(UnitManager.Instance.JumpUnit(unitName, power))
            : YarnTask.CompletedTask;
    }

    // Example: <<UnitMoveToPos "Ylru" 3 2>>
    [YarnCommand("UnitMoveToPos")]
    public static YarnTask UnitMoveToPos(string unitName, int x, int y)
    {
        return CutsceneManager.Instance != null
            ? CutsceneManager.Instance.YarnCoroutine(UnitManager.Instance.MoveUnitTo(unitName, new Vector2Int(x, y)))
            : YarnTask.CompletedTask;
    }

    // Example: <<UnitEmote "Ylru" "Gay" 2>>
    [YarnCommand("UnitEmote")]
    public static YarnTask UnitEmote(string unitName, string emote, float duration)
    {
        return CutsceneManager.Instance != null
            ? CutsceneManager.Instance.YarnCoroutine(UnitManager.Instance.EmoteUnit(unitName, emote, duration))
            : YarnTask.CompletedTask;
    }

    // Example: <<Wait 1.5>>
    [YarnCommand("Wait")]
    public static YarnTask Wait(float duration)
    {
        return CutsceneManager.Instance != null
            ? CutsceneManager.Instance.YarnCoroutine(CutsceneManager.Instance.Wait(duration))
            : YarnTask.CompletedTask;
    }
}
