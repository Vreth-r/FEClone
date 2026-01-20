using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CampPlayerController : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    private Rigidbody2D rb;
    public Animator animator;
    private Vector2 movement;
    private Vector2 lastMoveDir = Vector2.left;
    private CampInteractable currentInteractable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (ControlsManager.Instance != null)
        {
            ControlsManager.Instance.OnInteract += HandleInteract;
        }
    }

    private void Update()
    {
        if (CampInputBlocker.Blocked)
        {
            SetIdle();
            return;
        }
        movement.x = Input.GetAxisRaw("Horizontal"); // holy fuck this was hardcoded the whole fucking time?
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        UpdateAnimation();
    }

    private void HandleInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void FixedUpdate()
    {
        if (CampInputBlocker.Blocked)
        {
            rb.linearVelocity = new Vector2(0, 0);
            return;
        }
        rb.linearVelocity = movement * moveSpeed;
    }

    private void UpdateAnimation()
    {
        bool isMoving = movement.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            lastMoveDir = movement;
            UpdateFacing(movement);
        }
        else
        {
            UpdateFacing(lastMoveDir);
        }
    }

    private void UpdateFacing(Vector2 dir)
    {
        Vector3 faceRight = new Vector3(-1f, 1f, 1f);
        Vector3 faceLeft  = new Vector3( 1f, 1f, 1f);

        // left or up is left
        if (dir.x < 0f || dir.y > 0f)
        {
            transform.localScale = faceLeft;
        }
        // right or down is right
        else if (dir.x > 0f || dir.y < 0f)
        {
            transform.localScale = faceRight;
        }
    }


    private void SetIdle()
    {
        animator.SetBool("IsMoving", false);
        UpdateFacing(lastMoveDir);
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
