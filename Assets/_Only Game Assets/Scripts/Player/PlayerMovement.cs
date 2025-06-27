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
    public GameObject parry;

    public float moveSpeed = 5f;
    public float attackCooldown = 0.15f;
    public float defenseCooldown = 0.5f;
    public float attackCooldownTimer = 0;
    public float defenseCooldownTimer = 0;
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
        if (defenseCooldownTimer <= 0)
        {
            Defense();
            defense = true;
            defenseCooldownTimer = defenseCooldown;
        }
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
        float timeScale = Time.timeScale > 0 ? Time.timeScale : 1f;
        rb.linearVelocity = moveInput * moveSpeed / timeScale;
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
        attackCooldownTimer -= Time.unscaledDeltaTime;
        defenseCooldownTimer -= Time.unscaledDeltaTime;
        attack.transform.localScale = Vector3.one * SpecialAbilityManager.GetResource(ResourceTypes.AttackingRadius).amount;
        SetAttackAndParryRot(velocity);
    }

    private void SetAttackAndParryRot(Vector2 vel)
    {
        if (vel.x > 0)
        {
            attack.transform.eulerAngles = new(0, 0, 0);
            parry.transform.eulerAngles = new(0, 0, 0);
        }
        if (vel.x < 0)
        {
            attack.transform.eulerAngles = new(0, 0, 180);
            parry.transform.eulerAngles = new(0, 0, 180);
        }
        if (vel.y < 0)
        {
            attack.transform.eulerAngles = new(0, 0, -90);
            parry.transform.eulerAngles = new(0, 0, -90);
        }
        if (vel.y > 0)
        {
            attack.transform.eulerAngles = new(0, 0, 90);
            parry.transform.eulerAngles = new(0, 0, 90);
        }
    }

    private Coroutine disableAttackCoroutine;

    private void Attack()
    {
        attack.SetActive(true);

        if (disableAttackCoroutine != null)
            StopCoroutine(disableAttackCoroutine); // Prevent overlap

        disableAttackCoroutine = StartCoroutine(DisableIt(attack, 0.1f));
    }

    private IEnumerator DisableIt(GameObject obj, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        obj.SetActive(false);
    }

    private Coroutine disableParryCoroutine;
    private void Defense()
    {
        parry.SetActive(true);

        if (disableParryCoroutine != null)
        {
            StopCoroutine(disableParryCoroutine);
        }

        disableParryCoroutine = StartCoroutine(DisableIt(parry, 0.5f));
    }
}
