using UnityEngine;
using Unity.Netcode;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private RectTransform HealthUI; // Reference to health UI element
    private NetworkHealthState networkHealth;

    private void OnEnable()
    {
        Debug.Log("Inside OnEnable on PlayerUI");

        // Attempt to get the NetworkHealthState component
        networkHealth = GetComponent<NetworkHealthState>();
        
        // Ensure networkHealth is assigned and subscribe to the event if it's not null
        if (networkHealth != null)
        {
            networkHealth.HealthPoint.OnValueChanged += HealthChanged;
        }
        else
        {
            Debug.LogError("NetworkHealthState not found on this GameObject!");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when disabling the script
        if (networkHealth != null)
        {
            networkHealth.HealthPoint.OnValueChanged -= HealthChanged;
        }
    }

    // Called when health changes
    private void HealthChanged(int previousValue, int newValue)
    {
        Debug.Log("Inside HealthChanged on PlayerUI");
        // Update the health bar based on the new health value
        UpdateHealthUI(newValue);
    }

    // Updates the health UI scale
    public void UpdateHealthUI(int newValue)
    {
        Debug.Log("Inside UpdateHealthUI on PlayerUI");
        // Calculate the health ratio (ensure it doesn't go below 0 or above 1)
        float healthRatio = Mathf.Clamp01((float)newValue / 100f);  // Prevent negative scale or scale > 1
        // Update the scale of the health UI
        HealthUI.localScale = new Vector3(healthRatio, 1, 1);
        Debug.Log($"Health updated to {newValue}. New Scale: {HealthUI.localScale.x}");
    }
}
