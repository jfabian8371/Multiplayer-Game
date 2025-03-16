using Unity.Netcode;
using UnityEngine;

public class PlayerMovementTwo : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!IsOwner) return; // Only allow movement for the local player

        float moveX = 0f;
        float moveZ = 0f;

        // Assign controls based on player ID
        if (OwnerClientId == 0) // Player1 (WASD)
        {
            moveX = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f;
            moveZ = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;
        }
        else if (OwnerClientId == 1) // Player2 (Arrow Keys)
        {
            moveX = Input.GetKey(KeyCode.LeftArrow) ? -1f : Input.GetKey(KeyCode.RightArrow) ? 1f : 0f;
            moveZ = Input.GetKey(KeyCode.UpArrow) ? 1f : Input.GetKey(KeyCode.DownArrow) ? -1f : 0f;
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Handle gravity and jumping
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small value to keep grounded
        }

        if ((OwnerClientId == 0 && Input.GetKeyDown(KeyCode.Space)) || 
            (OwnerClientId == 1 && Input.GetKeyDown(KeyCode.RightControl)))
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
