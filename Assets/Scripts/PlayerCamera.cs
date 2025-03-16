using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    public Transform target; // Player's transform
    public float smoothSpeed = 10f; // Mouse sensitivity

    public Camera playerCamera;
    private float verticalRotation = 0f; // Track vertical rotation

    void Start()
    {
        if (IsOwner) // Check if the current client is the owner of the object
        {
            playerCamera.gameObject.SetActive(true); // Enable the local player's camera
        }
        else
        {
            playerCamera.gameObject.SetActive(false); // Disable other cameras
        }
    }
    void Update()
    {
        if (target == null) return;
        if (IsOwner) // Only allow the local player to move the camera
        {
            // Logic for camera to follow the local player
            // For example, make the camera follow the player’s position:
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z); // Adjust based on player position
        }
        float mouseX = Input.GetAxis("Mouse X") * smoothSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * smoothSpeed * Time.deltaTime;

        // Rotate the player horizontally
        target.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically (looking up/down)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); // Limit up/down movement
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
