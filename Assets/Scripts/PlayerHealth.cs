using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;

public class PlayerHealth : NetworkBehaviour
{
    public float maxHealth = 100f;
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    private Text healthText;
    [SerializeField] private AudioClip damageSound; // Damage sound effect
    [SerializeField] private AudioClip deathSound; // Death sound effect
    [SerializeField] private AudioSource audioSource; // AudioSource component

    void Start()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            healthText = GameObject.Find("HealthText")?.GetComponent<Text>();
            UpdateHealthUI();
        }

        // 🔹 Ensure the AudioSource is not null
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        currentHealth.OnValueChanged += (prev, current) =>
        {
            Debug.Log($"Health changed from {prev} to {current}");
            UpdateHealthUI();
        };
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        if (currentHealth.Value > 0)
        {
            Debug.Log($"Server: Applying {damage} damage. Current health before: {currentHealth.Value}");
            currentHealth.Value -= damage;
            Debug.Log($"Server: New health after damage: {currentHealth.Value}");

            // 🔹 Play damage sound on all clients
            PlayDamageSoundClientRpc();

            if (currentHealth.Value <= 0)
            {
                Die();
            }
        }
    }

    [ClientRpc]
    private void PlayDamageSoundClientRpc()
    {
        if (IsClient && audioSource != null && damageSound != null) //  Only play on clients
        {
            audioSource.PlayOneShot(damageSound);
            Debug.Log("Playing damage sound effect.");
        }
    }

    private void Die()
    {
        // 🔹 Play death sound for all clients
        PlayDeathSoundClientRpc();

        Debug.Log("Player has died.");
        StartCoroutine(DelayedDespawn());
        // if (IsServer) // 🔹 Ensure only the server despawns the player
        // {
        //     NetworkObject networkObject = GetComponent<NetworkObject>();
        //     if (networkObject != null)
        //     {
        //         Debug.Log("Despawning player: " + gameObject.name);
        //         networkObject.Despawn(true); // True removes from all clients
        //     }
        //     else
        //     {
        //         Debug.LogWarning("NetworkObject not found on player!");
        //     }
        // }
    }

    // 🔹 Play death sound for all clients
    [ClientRpc]
    private void PlayDeathSoundClientRpc()
    {
        if (IsClient && audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
            Debug.Log("Playing death sound effect.");
        }else
        {
        Debug.LogWarning("Either audioSource or deathSound is missing!");
        }
    }

    private IEnumerator DelayedDespawn()
    {
        // Wait for the length of the death sound before despawning
        if (deathSound != null)
        {
            yield return new WaitForSeconds(deathSound.length); // Wait for the duration of the death sound
        }
        else
        {
            yield return new WaitForSeconds(1f); // Fallback delay if deathSound is null
        }

        if (IsServer) // Ensure only the server despawns the player
        {
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                Debug.Log("Despawning player: " + gameObject.name);
                networkObject.Despawn(true); // True removes from all clients
            }
            else
            {
                Debug.LogWarning("NetworkObject not found on player!");
            }
        }
    }

    private void UpdateHealthUI()
    {
        if (IsOwner && healthText != null)
        {
            healthText.text = "Health - " + currentHealth.Value.ToString("F0");
        }
    }

    // Add this method to PlayerHealth.cs
public void ApplyDamage(float damage)
{
    // This method is called directly on the server
    if (NetworkManager.Singleton.IsServer)
    {
        if (currentHealth.Value > 0)
        {
            Debug.Log($"Server: Applying {damage} damage directly. Current health before: {currentHealth.Value}");
            currentHealth.Value -= damage;
            Debug.Log($"Server: New health after damage: {currentHealth.Value}");
            
            // Play damage sound on all clients
            PlayDamageSoundClientRpc();
            
            if (currentHealth.Value <= 0)
            {
                Die();
            }
        }
    }
}
}
