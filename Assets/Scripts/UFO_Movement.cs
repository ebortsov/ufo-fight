using UnityEngine;
using UnityEngine.InputSystem;

public class UFO_Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveForce = 12f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float rotationSpeed = 120f;

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
    }

    private void Update()
    {
        // One press = one jump impulse.
        // Holding Space will NOT continue applying force.
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}