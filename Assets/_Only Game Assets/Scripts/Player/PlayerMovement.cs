using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction moveAction, attackAction, defenseAction;
    private Vector2 lastVelocity;
    private Vector2 moveInput;
    private Animator animator;

    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private bool moving, attacking, defense;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        var actionMap = inputActions.FindActionMap("Player");
        moveAction = actionMap.FindAction("Move");
        attackAction = actionMap.FindAction("Attack");
        defenseAction = actionMap.FindAction("Defense");

        moveAction.Enable();
        attackAction.Enable();

        moveAction.performed += ctx =>
        {
            moveInput = ctx.ReadValue<Vector2>();
            moving = true;
        };
        moveAction.canceled += ctx =>
        {
            moveInput = Vector2.zero;
            moving = false;
        };

        attackAction.performed += ctx =>
        {
            Attack();
            attacking = true;
        };
        attackAction.canceled += ctx =>
        {
            attacking = false;
        };
        defenseAction.performed += ctx =>
        {
            Defense();
            defense = true;
        };
        defenseAction.canceled += ctx =>
        {
            defense = false;
        };
    }

    private void OnDisable()
    {
        moveAction.Disable();
        attackAction.Disable();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
    public void SetAnimParameters(Vector2 velocity)
    {
        if (velocity == Vector2.zero)
        {
            velocity = lastVelocity;
        }
        lastVelocity = velocity;
        animator.SetBool("moving", moving);
        animator.SetBool("attacking", attacking);
        animator.SetBool("defense", defense);
        animator.SetFloat("AnimMoveX", velocity.x);
        animator.SetFloat("AnimMoveY", velocity.y);
    }
    private void Update()
    {
        moveSpeed = SpecialAbilityManager.GetResource(ResourceTypes.MovementSpeed).amount;
        Vector2 velocity = rb.linearVelocity;
        SetAnimParameters(velocity);
    }
    private void Attack()
    {
        // Add attack logic here
        Debug.Log("Attack performed!");
    }

    private void Defense()
    {
        // Add defense logic here
        Debug.Log("Defense performed!");
    }
}
