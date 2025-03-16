using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public int playerNumber; // 1 for Player 1, 2 for Player 2
    private float moveSpeed = 5f;

    void Update()
    {
        Vector3 movement = Vector3.zero;

        if (playerNumber == 1)
        {
            // Player 1 uses WASD
            if (Input.GetKey(KeyCode.W)) movement.y = 1;
            if (Input.GetKey(KeyCode.S)) movement.y = -1;
            if (Input.GetKey(KeyCode.A)) movement.x = -1;
            if (Input.GetKey(KeyCode.D)) movement.x = 1;
        }
        else if (playerNumber == 2)
        {
            // Player 2 uses Arrow keys
            if (Input.GetKey(KeyCode.UpArrow)) movement.y = 1;
            if (Input.GetKey(KeyCode.DownArrow)) movement.y = -1;
            if (Input.GetKey(KeyCode.LeftArrow)) movement.x = -1;
            if (Input.GetKey(KeyCode.RightArrow)) movement.x = 1;
        }

        // Apply movement
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}
