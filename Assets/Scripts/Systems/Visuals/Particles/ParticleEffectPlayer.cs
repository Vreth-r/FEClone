using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ParticleEffectPlayer : MonoBehaviour
{
    public IEnumerator PlaySequence(ParticleEffectSequence sequence, Vector3 origin, Vector3 target)
    {
        var groups = sequence.steps
            .GroupBy(s => s.groupID)
            .OrderBy(g => g.Key);

        foreach (var group in groups)
        {
            var running = new List<IEnumerator>();
            foreach (var step in group)
                running.Add(PlayStep(step, origin, target));

            yield return StartCoroutine(WaitForAll(running));
        }

        Destroy(gameObject); // doing a sequence per player is actually a lot easier than doing queues and shit, its just a bit heavier on performance
        // but not by a significant margin.
    }

    // this is semantically a parser
    private IEnumerator PlayStep(ParticleEffectStep step, Vector3 origin, Vector3 target)
    {
        yield return new WaitForSeconds(step.delay);

        if (step.isTravelEffect)
        {
            // Spawn projectile at origin, move toward target
            var fx = Instantiate(step.effectPrefab, origin, Quaternion.identity, gameObject.transform);
            var mover = fx.AddComponent<ParticleMover>();
            bool done = false;

            mover.OnFinished += () => done = true;
            if (step.destination == "origin")
            {
                mover.Init(origin, step.speed);
            }
            else
            {
                mover.Init(target, step.speed);
            }

            yield return new WaitUntil(() => done);
        }
        else
        {
            GameObject fx;
            // Instant effect at target
            if (step.destination == "origin")
            {
                fx = Instantiate(step.effectPrefab, origin, Quaternion.identity, gameObject.transform);
            }
            else
            {
                fx = Instantiate(step.effectPrefab, target, Quaternion.identity, gameObject.transform);
            }
            Destroy(fx, step.lifetime);
            yield return new WaitForSeconds(step.lifetime);
        }
    }

    private IEnumerator WaitForAll(List<IEnumerator> routines)
    {
        bool[] finished = new bool[routines.Count];
        for (int i = 0; i < routines.Count; i++)
        {
            int idx = i;
            StartCoroutine(RunAndMark(routines[i], () => finished[idx] = true));
        }

        yield return new WaitUntil(() => finished.All(f => f));
    }

    private IEnumerator RunAndMark(IEnumerator routine, System.Action onDone)
    {
        yield return StartCoroutine(routine);
        onDone?.Invoke();
    }
}
