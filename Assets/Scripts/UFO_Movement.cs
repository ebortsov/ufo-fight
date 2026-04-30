using UnityEngine;
using UnityEngine.InputSystem;

public class UFO_Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveForce = 12f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float jumpCooldown = 0.35f;
    private float nextJumpTime = 0f;    

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Forward / backward movement
        if (Keyboard.current.wKey.isPressed)
        {
            rb.AddForce(transform.forward * moveForce, ForceMode.Force);
        }

        if (Keyboard.current.sKey.isPressed)
        {
            rb.AddForce(-transform.forward * moveForce, ForceMode.Force);
        }

        // Rotation with A / D
        float rotationInput = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            rotationInput = -1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            rotationInput = 1f;
        }

        Quaternion rotationDelta = Quaternion.Euler(
            0f,
            rotationInput * rotationSpeed * Time.fixedDeltaTime,
            0f
        );

        rb.MoveRotation(rb.rotation * rotationDelta);

        Vector3 velocity = rb.linearVelocity;

        // Clamp horizontal speed (ignore Y so jumping still works)
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 limited = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, velocity.y, limited.z);
        }
    }

    private void Update()
    {
        // One press = one jump impulse.
        // Holding Space will NOT continue applying force.
        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time >= nextJumpTime)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            nextJumpTime = Time.time + jumpCooldown;
        }
    }
}