using UnityEngine;
using Unity.Netcode;

public class HealthBarUI : NetworkBehaviour
{
    [SerializeField] private RectTransform Health2;  // Assign the Health2 UI bar in the Inspector
    private NetworkHealthState networkHealth;

    private void Start()
    {
        networkHealth = GetComponent<NetworkHealthState>();

        if (networkHealth != null)
        {
            networkHealth.HealthPoint.OnValueChanged += HealthChanged;
        }
    }

    private void OnDestroy()
    {
        if (networkHealth != null)
        {
            networkHealth.HealthPoint.OnValueChanged -= HealthChanged;
        }
    }

    private void HealthChanged(int previousValue, int newValue)
    {
        UpdateHealthUI(newValue);
    }

    public void UpdateHealthUI(int newValue)
    {
        float healthRatio = Mathf.Clamp01((float)newValue / 100f);  // Normalize health between 0 and 1
        Health2.localScale = new Vector3(healthRatio, 1, 1);

        Debug.Log($"Health2 UI Updated: {newValue} HP -> Scale X: {Health2.localScale.x}");
    }
}
