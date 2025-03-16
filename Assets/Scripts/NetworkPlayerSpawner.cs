using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    public GameObject playerPrefab; // The player prefab with NetworkObject
    public Transform spawnPoint1;  // Assign this in the Unity Editor for Player 1
    public Transform spawnPoint2;  // Assign this in the Unity Editor for Player 2

    private Transform[] spawnPoints; // Array to store spawn locations
    private int nextSpawnIndex = 0; // Tracks the next available spawn position
    private int playerCount = 0; // Keep track of the number of connected players

    [Header("Sound Effects")]
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private GameObject audioPlayerPrefab; // Prefab with AudioPlayer component

    private void Start()
    {
        // Store the spawn points in an array
        spawnPoints = new Transform[] { spawnPoint1, spawnPoint2 };
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) // Only the server handles player spawning
        {
            NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        Debug.Log($"Spawning player for Client ID: {clientId}");

        if (nextSpawnIndex < spawnPoints.Length && spawnPoints[nextSpawnIndex] != null)
        {
            Vector3 spawnPosition = spawnPoints[nextSpawnIndex].position;
            SpawnPlayerObject(clientId, spawnPosition);
            nextSpawnIndex++; // Move to next spawn position
        }
        else
        {
            Debug.LogError("Max player limit reached or spawn points not assigned!");
        }

        // Increment player count and assign player number dynamically
        playerCount++;
        AssignPlayerNumber(clientId, playerCount);
    }

    private void SpawnPlayerObject(ulong clientId, Vector3 spawnPosition)
    {
        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            networkObject.SpawnAsPlayerObject(clientId);
            Debug.Log($"Player {clientId} spawned at {spawnPosition}");
            
            // Play spawn sound for all clients
            StartCoroutine(DelayedSpawnSound(spawnPosition));
            
            // Ensure only the local player gets their camera enabled
            if (NetworkManager.Singleton.LocalClientId == clientId)
            {
                AttachCamera(playerInstance);
            }
        }
        else
        {
            Debug.LogError("Failed to spawn player: Missing NetworkObject component!");
        }
    }
    private IEnumerator DelayedSpawnSound(Vector3 position)
    {
        // Wait for 3 seconds before playing the spawn sound
        yield return new WaitForSeconds(3f);

        // Play the spawn sound after the delay
        PlaySpawnSoundClientRpc(position);
    }

[ClientRpc]
private void PlaySpawnSoundClientRpc(Vector3 position)
{
    if (IsClient && spawnSound != null)
    {
        Debug.Log($"Client {NetworkManager.Singleton.LocalClientId}: Attempting to play spawn sound");
        
        if (audioPlayerPrefab != null)
        {
            // Create a standalone audio player at the spawn position
            GameObject audioPlayerObj = Instantiate(audioPlayerPrefab, position, Quaternion.identity);
            AudioPlayer audioPlayer = audioPlayerObj.GetComponent<AudioPlayer>();
            
            if (audioPlayer != null)
            {
                audioPlayer.PlayAndDestroy(spawnSound);
                Debug.Log($"Spawn sound should be playing now. Sound length: {spawnSound.length}s");
            }
            else
            {
                // If AudioPlayer component is missing, add it
                audioPlayer = audioPlayerObj.AddComponent<AudioPlayer>();
                audioPlayer.PlayAndDestroy(spawnSound);
                Debug.LogWarning("Added AudioPlayer component to prefab and played sound");
            }
        }
        else
        {
            // Fallback if prefab is missing - play at position
            AudioSource.PlayClipAtPoint(spawnSound, position, 1.0f);
            Debug.LogWarning("AudioPlayerPrefab missing - using PlayClipAtPoint fallback");
        }
    }
}

    private void AttachCamera(GameObject player)
    {
        Camera playerCamera = player.GetComponentInChildren<Camera>();

        if (playerCamera != null)
        {
            // Enable the camera
            playerCamera.gameObject.SetActive(true);
        
            // Ensure the camera has an audio listener
            AudioListener audioListener = playerCamera.GetComponent<AudioListener>();
            if (audioListener == null)
                {
                audioListener = playerCamera.gameObject.AddComponent<AudioListener>();
                Debug.Log("Added AudioListener to player camera");
            }
            else
            {
                audioListener.enabled = true;
                Debug.Log("Enabled existing AudioListener on player camera");
            }
        
            // Disable the main camera's audio listener if it exists
            Camera mainCamera = Camera.main;
            if (mainCamera != null && mainCamera != playerCamera)
            {
                AudioListener mainListener = mainCamera.GetComponent<AudioListener>();
                if (mainListener != null)
                {
                    mainListener.enabled = false;
                    Debug.Log("Disabled main camera's AudioListener");
                }
            }
        
            PlayerCamera cameraScript = playerCamera.GetComponent<PlayerCamera>();
            if (cameraScript != null)
            {
                cameraScript.enabled = true;
            }
        }
        else
        {
            Debug.LogError("Camera not found on player object.");
        }
}

    // Assign player number dynamically based on connection order
    private void AssignPlayerNumber(ulong clientId, int playerNumber)
    {
        GameObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).gameObject;
        
        // Assuming you have a PlayerController script that holds the playerNumber
        PlayerController controller = playerObject.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.playerNumber = playerNumber;
            Debug.Log($"Assigned Player Number {playerNumber} to Client {clientId}");
        }
        else
        {
            Debug.LogError("PlayerController script not found on player object.");
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
        }
    }
}