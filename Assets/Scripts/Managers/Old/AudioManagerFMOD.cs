using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AudioManagerFMOD : MonoBehaviour
{
    public static AudioManagerFMOD Instance { get; private set; }

    [System.Serializable]
    public struct FmodEventEntry
    {
        public string id; // e.g. "UI_Click", "BGM_Main"
        public EventReference eventRef; // drag FMOD event here
    }

    [Header("FMOD Events (assign in Inspector)")]
    [SerializeField] private List<FmodEventEntry> events = new();

    // Quick lookup: id -> EventReference
    private Dictionary<string, EventReference> _eventMap;

    // Tracked instances: instanceKey -> instance
    private readonly Dictionary<string, EventInstance> _instances = new();

    // Optional: if following transforms, we keep a map
    private readonly Dictionary<string, Transform> _followTargets = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildEventMap();
    }

    private void OnEnable()
    {
        AudioEvents.PlayOneShotRequested += OnPlayOneShotRequested;
        AudioEvents.PlayInstanceRequested += OnPlayInstanceRequested;
        AudioEvents.StopInstanceRequested += OnStopInstanceRequested;
        AudioEvents.SetInstanceParamRequested += OnSetInstanceParamRequested;
    }

    private void OnDisable()
    {
        AudioEvents.PlayOneShotRequested -= OnPlayOneShotRequested;
        AudioEvents.PlayInstanceRequested -= OnPlayInstanceRequested;
        AudioEvents.StopInstanceRequested -= OnStopInstanceRequested;
        AudioEvents.SetInstanceParamRequested -= OnSetInstanceParamRequested;
    }

    private void OnDestroy()
    {
        // Ensure release everything on teardown
        foreach (var kvp in _instances)
        {
            if (kvp.Value.isValid())
            {
                kvp.Value.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                kvp.Value.release();
            }
        }
        _instances.Clear();
        _followTargets.Clear();
    }

    private void Update()
    {
        // Update 3D attributes for followed instances
        if (_followTargets.Count == 0) return;

        // Make a temp list to avoid modifying collection during iteration
        List<string> deadKeys = null;

        foreach (var kvp in _followTargets)
        {
            var key = kvp.Key;
            var t = kvp.Value;

            if (!_instances.TryGetValue(key, out var inst) || !inst.isValid() || t == null)
            {
                deadKeys ??= new List<string>();
                deadKeys.Add(key);
                continue;
            }

            inst.set3DAttributes(RuntimeUtils.To3DAttributes(t));
        }

        if (deadKeys != null)
        {
            foreach (var k in deadKeys)
                _followTargets.Remove(k);
        }
    }

    private void BuildEventMap()
    {
        _eventMap = new Dictionary<string, EventReference>(events.Count);
        foreach (var e in events)
        {
            if (string.IsNullOrWhiteSpace(e.id)) continue;

            if (_eventMap.ContainsKey(e.id))
            {
                Debug.LogWarning($"[AudioManagerFMOD] Duplicate event id '{e.id}' on {name}. Keeping first.");
                continue;
            }

            _eventMap.Add(e.id, e.eventRef);
        }
    }

    private bool TryGetEvent(string eventId, out EventReference eventRef)
    {
        if (_eventMap == null) BuildEventMap();

        if (_eventMap.TryGetValue(eventId, out eventRef))
            return true;

        Debug.LogWarning($"[AudioManagerFMOD] Unknown eventId '{eventId}'. Did you add it to AudioManagerFMOD?");
        return false;
    }

    // ---------------- Event handlers ----------------

    private void OnPlayOneShotRequested(string eventId, Vector3? position, Transform follow)
    {
        if (!TryGetEvent(eventId, out var ev)) return;

        if (follow != null)
        {
            RuntimeManager.PlayOneShotAttached(ev, follow.gameObject);
            return;
        }

        if (position.HasValue)
        {
            RuntimeManager.PlayOneShot(ev, position.Value);
            return;
        }

        // 2D one-shot
        RuntimeManager.PlayOneShot(ev);
    }

    private void OnPlayInstanceRequested(string instanceKey, string eventId, Vector3? position, Transform follow)
    {
        if (string.IsNullOrWhiteSpace(instanceKey))
        {
            Debug.LogWarning("[AudioManagerFMOD] instanceKey is null/empty. Provide a key to track & stop later.");
            return;
        }

        if (!TryGetEvent(eventId, out var ev)) return;

        // If already playing under this key, stop + replace
        StopInstanceInternal(instanceKey, allowFadeOut: true);

        var inst = RuntimeManager.CreateInstance(ev);

        // 3D positioning / following
        if (follow != null)
        {
            inst.set3DAttributes(RuntimeUtils.To3DAttributes(follow));
            _followTargets[instanceKey] = follow;
        }
        else if (position.HasValue)
        {
            inst.set3DAttributes(RuntimeUtils.To3DAttributes(position.Value));
            _followTargets.Remove(instanceKey);
        }
        else
        {
            _followTargets.Remove(instanceKey);
        }

        inst.start();

        _instances[instanceKey] = inst;
    }

    private void OnStopInstanceRequested(string instanceKey, bool allowFadeOut)
    {
        StopInstanceInternal(instanceKey, allowFadeOut);
    }

    private void StopInstanceInternal(string instanceKey, bool allowFadeOut)
    {
        if (!_instances.TryGetValue(instanceKey, out var inst) || !inst.isValid())
        {
            _instances.Remove(instanceKey);
            _followTargets.Remove(instanceKey);
            return;
        }

        inst.stop(allowFadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        inst.release();

        _instances.Remove(instanceKey);
        _followTargets.Remove(instanceKey);
    }

    private void OnSetInstanceParamRequested(string instanceKey, string paramName, float value)
    {
        if (!_instances.TryGetValue(instanceKey, out var inst) || !inst.isValid())
            return;

        inst.setParameterByName(paramName, value);
    }
}