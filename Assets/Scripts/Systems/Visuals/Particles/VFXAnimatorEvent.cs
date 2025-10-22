using UnityEngine;
using System;

// this is a bridge class
public class VFXAnimatorEvent : MonoBehaviour
{
    public event Action<string> OnVFXEvent;

    // gets called from animation event
    public void RaiseEvent(string eventName)
    {
        OnVFXEvent?.Invoke(eventName);
    }
}