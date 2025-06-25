using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private Vector2 moveInput;

    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        var actionMap = inputActions.FindActionMap("Player");
        moveAction = actionMap.FindAction("Move");

        moveAction.Enable();

        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;
    }

    private void Update()
    {
        moveSpeed = SpecialAbilityManager.GetResource(ResourceTypes.MovementSpeed).amount;
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
