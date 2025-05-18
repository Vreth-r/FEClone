using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CampPlayerController : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private CampInteractable currentInteractable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (CampInputBlocker.Blocked) return;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void FixedUpdate()
    {
        if (CampInputBlocker.Blocked)
        {
            rb.velocity = new Vector2(0, 0);
            return;
        }
        rb.velocity = movement * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out CampInteractable interactable))
        {
            currentInteractable = interactable;
            // show press e prompt here once u add that slime
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out CampInteractable interactable) && currentInteractable == interactable)
        {
            currentInteractable = null;
            // hide press e prompt
        }
    }
}
