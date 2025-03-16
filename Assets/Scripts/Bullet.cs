using Unity.Netcode;
using UnityEngine;
public class Bullet : NetworkBehaviour  // Changed to NetworkBehaviour
{
    public float damage = 10; // Set bullet damage
    public string playerTag = "Player"; // Tag to identify player objects
    public GameObject floorObject; // Reference to the floor GameObject
   
    public AudioClip shootSound; // The sound clip to play when bullet is shot
    private AudioSource audioSource; // AudioSource component
    private bool hasDealtDamage = false; // Flag to prevent multiple damage applications
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.1f;
        if (audioSource != null && shootSound != null){
            audioSource.PlayOneShot(shootSound);
        }
        Destroy(gameObject, 5f); // Destroy bullet after 5 seconds if it doesn't hit anything
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if the bullet collides with an object tagged as "Player"
        if (collision.gameObject.CompareTag(playerTag) && !hasDealtDamage)
        {
            // Only allow the server to apply damage, or use IsOwner if the bullet has an owner
            if (NetworkManager.Singleton.IsServer)
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    Debug.Log("Server: Player hit. Applying damage directly.");
                    hasDealtDamage = true; // Set flag to prevent multiple damage applications
                    
                    // Apply damage directly instead of using RPC since we're on the server
                    playerHealth.ApplyDamage(damage);
                }
                else
                {
                    Debug.LogWarning("PlayerHealth component not found on player.");
                }
            }
            
            // Destroy the bullet after it hits the player (on all clients)
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject);
        }
        // Check if the bullet collides with the floor
        else if (collision.gameObject == floorObject)
        {
            // Destroy the bullet if it hits the floor
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject);
        }
    }
}