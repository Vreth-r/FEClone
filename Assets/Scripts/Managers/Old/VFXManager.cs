using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }
    public ParticleEffectPlayer playerPrefab;

    void Awake() => Instance = this;

    public void PlayEffect(ParticleEffectSequence sequence, Vector3 origin, Vector3 target)
    {
        var player = Instantiate(playerPrefab, origin, Quaternion.identity);
        player.StartCoroutine(player.PlaySequence(sequence, origin, target));
    }
}
