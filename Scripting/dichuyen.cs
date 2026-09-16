using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Vector2 lastDirection = Vector2.right;
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    InputAction moveInput;
    InputAction JumpAction;
    Vector2 it;
    float recoilTime = 0;
    
    private void Awake()
    {
       rb = GetComponent<Rigidbody2D>();
       playerCollider = GetComponent<Collider2D>();
       moveInput = InputSystem.actions.FindAction("Move");
       JumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        it = moveInput.ReadValue<Vector2>();

        if(it.x > 0)
        {
            lastDirection = Vector2.right;
        }

        if(it.x < 0)
        {
            lastDirection = Vector2.left;
        }

        if(JumpAction.WasPressedThisFrame() && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
       if(recoilTime > 0)
       {
          recoilTime = recoilTime - Time.fixedDeltaTime;
          return;
       }

       rb.linearVelocity = new Vector2(it.x * moveSpeed, rb.linearVelocity.y);
    }

    public void PushBack(Vector2 shootDirection, float force)
    {
        rb.linearVelocity = new Vector2(-shootDirection.x * force, rb.linearVelocity.y);
        recoilTime = 0.12f;
    }

    bool IsGrounded()
    {
        Vector2 rayStart = new Vector2(transform.position.x, playerCollider.bounds.min.y - 0.02f);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 0.15f);
        return hit.collider != null;
    }
}
