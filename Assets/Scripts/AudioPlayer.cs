using UnityEngine;
using Unity.Netcode;

public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioSource added to AudioPlayer");
        }
    }
    
    public void PlayAndDestroy(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.volume = 1.0f; // Set appropriate volume
            audioSource.spatialBlend = 1.0f; // 3D sound (1.0) or 2D sound (0.0)
            audioSource.Play();
            
            // Log that we're playing the sound
            Debug.Log($"Playing sound: {clip.name} (length: {clip.length}s)");
            
            // Destroy this GameObject after the clip finishes playing
            Destroy(gameObject, clip.length + 0.1f);
        }
        else
        {
            Debug.LogError("Attempted to play null AudioClip");
            Destroy(gameObject);
        }
    }
}