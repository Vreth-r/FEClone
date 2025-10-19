using UnityEngine;
using System.Collections.Generic;

public class ParticleController : MonoBehaviour
{
    public static ParticleController Instance { get; private set; }

    [Header("Registered Particle Systems")]
    public ParticleSystem[] particleSystems;

    private Dictionary<string, ParticleSystem> particleDict = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var ps in particleSystems)
        {
            if (ps != null)
                particleDict[ps.name] = ps;
        }
    }

    public ParticleSystem GetParticleSystem(string name)
    {
        if (particleDict.TryGetValue(name, out var ps))
            return ps;
        Debug.LogWarning($"ParticleManager: No particle system named '{name}' found!");
        return null;
    }

    public void StartPS(string name)
    {
        ParticleSystem ps = GetParticleSystem(name);
        ps.Play();
        //Debug.Log($"[Yarn] Started particle system '{name}'");
    }

    public void StopPS(string name)
    {
        ParticleSystem ps = GetParticleSystem(name);
        ps.Stop();
    }
}
