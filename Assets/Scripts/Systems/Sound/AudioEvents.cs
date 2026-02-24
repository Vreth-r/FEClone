using System;
using UnityEngine;


// Im doing this properly now

public static class AudioEvents
{
    // One-shot (fire and forget). Optionally positioned.
    public static event Action<string, Vector3?, Transform> PlayOneShotRequested;

    // Create/Play a tracked instance. Returns nothing here; you track by instanceKey.
    public static event Action<string, string, Vector3?, Transform> PlayInstanceRequested;

    // Stop a tracked instance by instanceKey
    public static event Action<string, bool> StopInstanceRequested;

    // Set a parameter on a tracked instance by instanceKey
    public static event Action<string, string, float> SetInstanceParamRequested;

    // ---------- Convenience wrappers ----------
    public static void PlayOneShot(string eventId) =>
        PlayOneShotRequested?.Invoke(eventId, null, null);

    public static void PlayOneShotAt(string eventId, Vector3 position) =>
        PlayOneShotRequested?.Invoke(eventId, position, null);

    public static void PlayOneShotFollow(string eventId, Transform follow) =>
        PlayOneShotRequested?.Invoke(eventId, null, follow);

    public static void PlayInstance(string instanceKey, string eventId) =>
        PlayInstanceRequested?.Invoke(instanceKey, eventId, null, null);

    public static void PlayInstanceAt(string instanceKey, string eventId, Vector3 position) =>
        PlayInstanceRequested?.Invoke(instanceKey, eventId, position, null);

    public static void PlayInstanceFollow(string instanceKey, string eventId, Transform follow) =>
        PlayInstanceRequested?.Invoke(instanceKey, eventId, null, follow);

    public static void StopInstance(string instanceKey, bool allowFadeOut = true) =>
        StopInstanceRequested?.Invoke(instanceKey, allowFadeOut);

    public static void SetInstanceParam(string instanceKey, string paramName, float value) =>
        SetInstanceParamRequested?.Invoke(instanceKey, paramName, value);
}