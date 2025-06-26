using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction moveAction, attackAction, defenseAction;
    private Vector2 lastVelocity;
    private Vector2 moveInput;
    private Animator animator;
    public GameObject attack;

    public float moveSpeed = 5f;
    public float attackCooldown = 0.15f;
    public float attackCooldownTimer = 0;
    private Rigidbody2D rb;
    private bool moving, attacking, defense;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        moving = true;
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
        moving = false;
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (attackCooldownTimer <= 0)
        {
            Attack();
            attacking = true;
            attackCooldownTimer = attackCooldown;
        }
    }

    private void OnAttackCanceled(InputAction.CallbackContext ctx)
    {
        attacking = false;
    }

    private void OnDefense(InputAction.CallbackContext ctx)
    {
        Defense();
        defense = true;
    }

    private void OnDefenseCanceled(InputAction.CallbackContext ctx)
    {
        defense = false;
    }


    private void OnEnable()
    {
        var actionMap = inputActions.FindActionMap("Player");
        moveAction = actionMap.FindAction("Move");
        attackAction = actionMap.FindAction("Attack");
        defenseAction = actionMap.FindAction("Defense");

        moveAction.Enable();
        attackAction.Enable();
        defenseAction.Enable();

        moveAction.performed += OnMove;
        moveAction.canceled += OnMoveCanceled;

        attackAction.performed += OnAttack;
        attackAction.canceled += OnAttackCanceled;

        defenseAction.performed += OnDefense;
        defenseAction.canceled += OnDefenseCanceled;
    }


    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMoveCanceled;

        attackAction.performed -= OnAttack;
        attackAction.canceled -= OnAttackCanceled;

        defenseAction.performed -= OnDefense;
        defenseAction.canceled -= OnDefenseCanceled;

        moveAction.Disable();
        attackAction.Disable();
        defenseAction.Disable();
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
        attackCooldownTimer -= Time.deltaTime;
    }

    private Coroutine disableCoroutine;

    private void Attack()
    {
        attack.SetActive(true);

        if (disableCoroutine != null)
            StopCoroutine(disableCoroutine); // Prevent overlap

        disableCoroutine = StartCoroutine(DisableAttack());
    }

    private IEnumerator DisableAttack()
    {
        yield return new WaitForSeconds(0.1f);
        attack.SetActive(false);
        disableCoroutine = null;
    }

    private void Defense()
    {
        // Add defense logic here
        Debug.Log("Defense performed!");
    }
}
