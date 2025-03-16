using Unity.Netcode;
using UnityEngine;

public class SpawnSoundPlay : NetworkBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource
    [SerializeField] private AudioClip spawnSound; // The sound to play when the player spawns

    // This will be called when the player spawns or connects
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Only play sound for the local player (owner of the object)
        if (IsOwner)
        {
            PlaySpawnSound();
        }
    }

    private void PlaySpawnSound()
    {
        if (audioSource != null && spawnSound != null)
        {
            audioSource.clip = spawnSound;
            audioSource.Play();
            Debug.Log("Playing spawn sound");
        }
        else
        {
            Debug.LogError("AudioSource or SpawnSound not assigned!");
        }
    }
}
