using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    InputAction moveInput;
    InputAction AttackAction;
    InputAction JumpAction;
    Vector2 it;
    
    private void Awake()
    {
       rb = GetComponent<Rigidbody2D>();
       playerCollider = GetComponent<Collider2D>();
       moveInput = InputSystem.actions.FindAction("Move");
       AttackAction = InputSystem.actions.FindAction("Attack");
       JumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        it = moveInput.ReadValue<Vector2>();
         
        if(AttackAction.WasPressedThisFrame())
        {
            Debug.Log("Attack");
        }

        if(JumpAction.WasPressedThisFrame() && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
       rb.linearVelocity = new Vector2(it.x * moveSpeed, rb.linearVelocity.y);
    }

    bool IsGrounded()
    {
        Vector2 rayStart = new Vector2(transform.position.x, playerCollider.bounds.min.y - 0.02f);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 0.15f);
        return hit.collider != null;
    }
}
