using UnityEngine;

public class SquareNoise : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;   // Reference to AudioSource
    [SerializeField] private AudioClip interactionSound; // Sound effect to play when hit by a bullet
    private bool objectInRange = false; // Flag to check if any object is in range

    void OnTriggerEnter(Collider other)
    {
        // Detect if it's a bullet (you can set a specific tag or check for bullet components)
        
        
        PlayInteractionSound();
        
    }

    void OnTriggerExit(Collider other)
    {
        // Reset the flag when the object leaves the trigger area
        objectInRange = false;
    }

    private void PlayInteractionSound()
    {
        if (audioSource != null && interactionSound != null)
        {
            audioSource.clip = interactionSound;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("AudioSource or InteractionSound is missing.");
        }
    }
}
