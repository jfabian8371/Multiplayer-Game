using Unity.Netcode;
using UnityEngine;

public class NetworkHealthState : NetworkBehaviour
{
    [HideInInspector]
    public NetworkVariable<int> HealthPoint = new NetworkVariable<int>();

    [SerializeField] private Transform healthBar; // Reference to the health bar transform
    private float maxHealth = 100f; // Maximum health value

    public override void OnNetworkSpawn()
    {
        Debug.Log("Inside OnNetworkSpawn on NETWORKHEALTHSTATE");
        base.OnNetworkSpawn();
        if (IsServer) 
        {
            HealthPoint.Value = 100;
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Inside TakeDamage on NETWORKHEALTHSTATE");
        if (IsServer) // Ensure only the server modifies health
        {
            HealthPoint.Value -= damage;
            UpdateHealthBarClientRpc(HealthPoint.Value);
        }
    }

    [ClientRpc]
    private void UpdateHealthBarClientRpc(int newHealth)
    {
        Debug.Log("Inside UpdateHealthBarClientRpc on NETWORKHEALTHSTATE");
        if (healthBar != null)
        {
            Vector3 scale = healthBar.localScale;
            scale.x = Mathf.Clamp((float)newHealth / maxHealth, 0f, 1f); // Ensure scale is between 0 and 1
            healthBar.localScale = scale;
        }
        Debug.Log($"Health Bar Updated: {newHealth} HP");
    }
}
