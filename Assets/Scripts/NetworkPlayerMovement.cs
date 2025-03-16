using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 2f;
    private Rigidbody rb;

    public Transform playerRoot; // Main body of the player
    public Transform cameraHolder; // Assign this to the head/camera pivot
    private float verticalRotation = 0f; // Track vertical rotation (for camera only)

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource
    [SerializeField] private AudioClip footstepSound; // Footstep sound effect

    private bool isMoving = false; // To track movement state

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (IsOwner)
        {
            // Ensure zero initial velocity
            rb.linearVelocity = Vector3.zero;
            Camera.main.transform.SetParent(cameraHolder);
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;

            // Lock and hide cursor for FPS control
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        rb.freezeRotation = true; // Prevent physics-based rotation
    }

    void Update()
    {
        if (IsOwner)
        {
            HandleCameraRotation();
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                PlayFootstepSoundClientRpc(true);  // Start the footstep sound when any movement key is pressed
            }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
            {
                PlayFootstepSoundClientRpc(false);  // Stop the footstep sound when the movement key is released
            }
        }
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            // Handle movement in FixedUpdate for more stable physics
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        // Get raw input for precise movement
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D
        float moveZ = Input.GetAxisRaw("Vertical");   // W/S

        // Create movement direction based on player orientation
        Vector3 moveDirection = playerRoot.forward * moveZ + playerRoot.right * moveX;
        moveDirection.y = 0; // Ensure no vertical movement
        moveDirection.Normalize(); // Prevent faster diagonal movement

        // Check if player is grounded
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        if (isGrounded && (moveX != 0 || moveZ != 0))
        {
            // Apply movement force only in XZ plane
            Vector3 targetVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
            rb.linearVelocity = targetVelocity;

            // Play footstep sound if moving
            if (!isMoving)
            {
                isMoving = true;
                PlayFootstepSoundClientRpc(true); // Start playing sound
            }
        }
        else
        {
            // Maintain gravity influence and prevent floating
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

            // Stop footstep sound if not moving
            if (isMoving)
            {
                isMoving = false;
                PlayFootstepSoundClientRpc(false); // Stop playing sound
            }
        }
        
    }

    [ClientRpc]
private void PlayFootstepSoundClientRpc(bool play)
{
    Debug.Log("Footstep sound client RPC called: " + play);
    if (audioSource != null && footstepSound != null)
    {
        if (play)
        {
            if (!audioSource.isPlaying) // Prevent overlapping sound
            {
                audioSource.clip = footstepSound;
                audioSource.loop = true; // Enable looping
                audioSource.Play();
                Debug.Log("Playing footstep sound");
            }
        }
        else
        {
            audioSource.Stop();
            Debug.Log("Stopped footstep sound");
        }
    }
    else
    {
        Debug.LogError("AudioSource or Footstep Sound is missing.");
    }
}


    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Rotate player horizontally with mouse
        playerRoot.Rotate(Vector3.up * mouseX);

        // Handle vertical rotation (looking up/down) - camera only
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    // Debugging input issues
    void OnGUI()
    {
        if (IsOwner)
        {
            GUI.Label(new Rect(10, 10, 300, 20), "H: " + Input.GetAxis("Horizontal") + " V: " + Input.GetAxis("Vertical"));
            GUI.Label(new Rect(10, 30, 300, 20), "Velocity: " + rb.linearVelocity.ToString());
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if(!IsServer) return;
        if(collider.GetComponent<Bullet>()){
            GetComponent<NetworkHealthState>().HealthPoint.Value -= 10;
        }
    }
}
