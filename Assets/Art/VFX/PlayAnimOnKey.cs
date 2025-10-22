using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimOnKey : MonoBehaviour
{
    public ParticleEffectSequence sequence;
    public Transform origin;
    Vector3 target;

    void Start()
    {
        target = origin.position + new Vector3(5,0,0);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            VFXManager.Instance.PlayEffect(sequence, origin.position, target);
        }
    }
}
