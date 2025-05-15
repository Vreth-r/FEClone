using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CampPlayerController : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
    }

    private void FixedUpdate()
    {
        if(CampInputBlocker.Blocked) return;
        rb.velocity = movement * moveSpeed;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var interactable = other.GetComponent<CampInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}
