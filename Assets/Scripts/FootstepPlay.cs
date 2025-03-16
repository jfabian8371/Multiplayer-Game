using UnityEngine;

public class FootstepSoundPlay : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource
    [SerializeField] private AudioClip footstepSound; // Footstep sound effect

    private bool isPlayingFootsteps = false; // To track if the footstep sound is playing

    void Update()
    {
        HandleFootstepsSound();
    }

    private void HandleFootstepsSound()
    {
        // Check if the player is pressing any of the movement keys (W, A, S, D)
        bool isMoving = (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"));

        if (isMoving && !isPlayingFootsteps)
        {
            // Start playing footsteps if not already playing
            PlayFootstepSound(true);
            isPlayingFootsteps = true;
        }
        else if (!isMoving && isPlayingFootsteps)
        {
            // Stop playing footsteps if not moving
            PlayFootstepSound(false);
            isPlayingFootsteps = false;
        }
    }

    private void PlayFootstepSound(bool play)
    {
        if (audioSource != null && footstepSound != null)
        {
            if (play)
            {
                if (!audioSource.isPlaying) // Prevent overlapping sound
                {
                    audioSource.clip = footstepSound;
                    audioSource.loop = true; // Enable looping
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.Stop();
            }
        }
        else
        {
            Debug.LogError("AudioSource or Footstep Sound not assigned!");
        }
    }
}
