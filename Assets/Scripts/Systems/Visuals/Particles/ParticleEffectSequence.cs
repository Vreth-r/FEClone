using UnityEngine;

[CreateAssetMenu(menuName = "VFX/Particle Effect Sequence")]
public class ParticleEffectSequence : ScriptableObject
{
    public ParticleEffectStep[] steps;
}