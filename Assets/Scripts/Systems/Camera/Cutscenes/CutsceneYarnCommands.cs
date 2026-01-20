using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class CutsceneYarnCommands : MonoBehaviour
{
    // fires an event to the audio manager
    [YarnCommand("PlaySound")]
    public static void PlaySound(string soundId)
    {
        AudioEvents.Play(soundId);
    }

    // Loads a scene with the loading screen through the manager
    [YarnCommand("LoadScene")]
    public static void LoadSceneFromManager(string sceneName)
    {
        LoadingScreenManager.Instance.LoadScene(sceneName);
    }

    // to be fully made into saving
    [YarnCommand("SaveGame")]
    public static void SaveGame()
    {
        return;
    }
    
    [YarnCommand("FireEvent")]
    public static void FireEvent(string eventName)
    {
        // c# throws a fit if you dont wrap this
        CutsceneManager.Instance.FireEvent(eventName);
    }

    [YarnCommand("FlipTurn")]
    public static void FlipTurn()
    {
        TurnManager.Instance.TurnFlip();
    }

    [YarnCommand("SetLevelCompleteNode")]
    public static void SetLevelCompleteNode(string nodeName)
    {
        TurnManager.Instance.levelCompleteYarnNode = nodeName;
    }

    // this one feels wrong but im too lazy to find a better way when i might be changing control contexts soon
    [YarnCommand("SetControlContext")]
    public static void SetControlContext(string context)
    {
        if (context == "Gameplay")
        {
            ControlsManager.Instance.SetContext(InputContext.Gameplay);
        }
        else if (context == "Menu")
        {
            ControlsManager.Instance.SetContext(InputContext.Menu);
        }
        else if (context == "Cutscene")
        {
            ControlsManager.Instance.SetContext(InputContext.Cutscene);
        }
    }

    [YarnCommand("PlayAnim")]
    public static void PlayAnim(string key, string animName)
    {
        SceneAnimationController.Instance.PlayAnimation(key, animName);
    }

    // 0 is time stop, 1 is full speed
    [YarnCommand("SetAllAnimSpeed")]
    public static void SetAllAnimatorSpeeds(float time)
    {
        SceneAnimationController.Instance.SetAllAnimatorSpeeds(time);
    }

    // 0 is time stop, 1 is full speed
    [YarnCommand("SetAnimSpeed")]
    public static void SetAnimSpeed(string key, float time)
    {
        SceneAnimationController.Instance.SetAnimatorSpeed(key, time);
    }
    
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

    // non-blocking version (Yarn continues immediately)
    [YarnCommand("UnitMoveToPosNoBlock")]
    public static void UnitMoveToPosNoBlock(string unitName, int x, int y)
    {
        if (CutsceneManager.Instance == null)
            return;

        // yarn doesn’t wait for this
        CutsceneManager.Instance.StartCoroutine(
            UnitManager.Instance.MoveUnitTo(unitName, new Vector2Int(x, y))
        );
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

    [YarnCommand("FadeInBlack")]
    public static YarnTask FadeInBlack(float duration = 1f)
    {
        return CutsceneManager.Instance != null
            ? CutsceneManager.Instance.YarnCoroutine(CutsceneManager.Instance.FadeIn(duration))
            : YarnTask.CompletedTask;
    }

    [YarnCommand("FadeOutBlack")]
    public static YarnTask FadeOutBlack(float duration = 1f)
    {
        return CutsceneManager.Instance != null
            ? CutsceneManager.Instance.YarnCoroutine(CutsceneManager.Instance.FadeOut(duration))
            : YarnTask.CompletedTask;
    }

    [YarnCommand("ShowBlackText")]
    public static YarnTask ShowBlackText(string text, float duration = 2f)
    {
        return CutsceneManager.Instance != null
            ? CutsceneManager.Instance.YarnCoroutine(CutsceneManager.Instance.ShowText(text, duration))
            : YarnTask.CompletedTask;
    }

    [YarnCommand("CameraZoom")]
    public static YarnTask CameraZoom(float targetSize, float duration = 1f, float zoomSmoothTime = -1)
    {
        var panner = FindFirstObjectByType<CameraPanner>();
        if (panner == null)
        {
            Debug.LogWarning("CameraPanner not found in scene!");
            return YarnTask.CompletedTask;
        }

        return CutsceneManager.Instance.YarnCoroutine(panner.ZoomCamera(targetSize, duration, zoomSmoothTime));
    }

    [YarnCommand("CameraSetZoom")]
    public static void CameraSetZoom(float targetSize)
    {
        var panner = FindFirstObjectByType<CameraPanner>();
        if (panner != null)
            panner.SetZoom(targetSize);
    }

    [YarnCommand("StartParticles")]
    public static void StartParticles(string systemName)
    {
        ParticleController.Instance.StartPS(systemName);
    }

    [YarnCommand("StopParticles")]
    public static void StopParticles(string systemName)
    {
        ParticleController.Instance.StopPS(systemName);
    }
}
