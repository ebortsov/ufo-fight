using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUfoMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotationSpeed = 120f;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        enabled = IsOwner;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        HandleMovement();
    }

    private void HandleMovement()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        float rotateInput = 0f;
        float moveInput = 0f;

        if (keyboard.aKey.isPressed)
            rotateInput -= 1f;

        if (keyboard.dKey.isPressed)
            rotateInput += 1f;

        if (keyboard.wKey.isPressed)
            moveInput += 1f;

        if (keyboard.sKey.isPressed)
            moveInput -= 1f;

        transform.Rotate(
            Vector3.up,
            rotateInput * rotationSpeed * Time.deltaTime
        );

        Vector3 movement =
            transform.forward * moveInput * moveSpeed * Time.deltaTime;

        if (characterController != null)
        {
            characterController.Move(movement);
        }
        else
        {
            transform.position += movement;
        }


    }
}