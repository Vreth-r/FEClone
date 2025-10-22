using UnityEngine;
using System;

public class ParticleMover : MonoBehaviour
{
    private Vector3 targetPos;
    private float speed;
    private bool isActive;

    public event Action OnFinished;

    public void Init(Vector3 target, float moveSpeed)
    {
        targetPos = target;
        speed = moveSpeed;
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            isActive = false;
            OnFinished?.Invoke();
            Destroy(gameObject);
        }
    }
}
