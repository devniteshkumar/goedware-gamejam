using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private Vector2 moveInput;
    private Animator animator;

    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        var actionMap = inputActions.FindActionMap("Player");
        moveAction = actionMap.FindAction("Move");

        moveAction.Enable();

        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
    public void SetAnimParameters(Vector2 velocity)
    {
        bool idle = (velocity.x == 0 && velocity.y == 0);
        animator.SetBool("idle", idle);
        bool movingUp = velocity.y > 0;
        animator.SetBool("movingUp", movingUp);
        bool movingDown = velocity.y < 0;
        animator.SetBool("movingDown", movingDown);
        bool movingLeft = velocity.x < 0;
        animator.SetBool("movingLeft", movingLeft);
        bool movingRight = velocity.x > 0;
        animator.SetBool("movingRight", movingRight);
    }

    void Update()
    {
        Vector2 velocity = GetComponent<Rigidbody2D>().linearVelocity;
        SetAnimParameters(velocity);
    }
}
