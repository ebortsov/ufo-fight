using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUfoMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotationSpeed = 120f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravityMultiplier = 3.5f;

    private Rigidbody rb;

    private float moveInput;
    private float rotateInput;
    private bool jumpPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        enabled = IsOwner;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        ReadInput();
    }

    private void FixedUpdate()
    {
        if (!IsOwner || rb == null)
            return;

        MoveUfo();
        Jump();
        ApplyExtraGravity();
    }

    private void ReadInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        rotateInput = 0f;
        moveInput = 0f;

        if (keyboard.aKey.isPressed)
            rotateInput -= 1f;

        if (keyboard.dKey.isPressed)
            rotateInput += 1f;

        if (keyboard.wKey.isPressed)
            moveInput += 1f;

        if (keyboard.sKey.isPressed)
            moveInput -= 1f;

        if (keyboard.spaceKey.wasPressedThisFrame)
            jumpPressed = true;
    }

    private void MoveUfo()
    {
        Quaternion rotationDelta =
            Quaternion.Euler(0f, rotateInput * rotationSpeed * Time.fixedDeltaTime, 0f);

        rb.MoveRotation(rb.rotation * rotationDelta);

        Vector3 movement =
            transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movement);
    }

    private void Jump()
    {
        if (!jumpPressed)
            return;

        jumpPressed = false;

        Vector3 velocity = rb.linearVelocity;

        if (velocity.y < 0f)
        {
            velocity.y = 0f;
            rb.linearVelocity = velocity;
        }

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ApplyExtraGravity()
    {
        rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
    }
}