using UnityEngine;

public class PlayerMP7 : AWeapon
{
    public float damage = 10f; // Damage dealt by the weapon
    public float range = 50f; // Range of the weapon
    public Transform shootingPoint; // The point where the bullet or ray is fired (usually the front of the weapon)
    public GameObject bulletPrefab; // Bullet prefab (optional, if you want a projectile)
    public float bulletSpeed = 20f; // Speed of the bullet

    public override void Fire()
    {
        Debug.Log("Fire!");

        // Perform a raycast to simulate shooting
        RaycastHit hit;
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // Check if the hit object has a PlayerHealth script (to take damage)
            PlayerHealth targetHealth = hit.collider.GetComponent<PlayerHealth>();
            if (targetHealth != null)
            {
                // Apply damage to the target
                targetHealth.TakeDamageServerRpc(damage);
            }
        }

        // Optionally instantiate a bullet (for projectile-based shooting)
        if (bulletPrefab != null)
        {
            // Instantiate the bullet at the shooting point
            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);

            // Get the Rigidbody component of the bullet and apply velocity
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                // Set the velocity of the bullet in the direction of the shooting point's forward direction
                bulletRb.linearVelocity = shootingPoint.forward * bulletSpeed;
            }
        }
    }
}
