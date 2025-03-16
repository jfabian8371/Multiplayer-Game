using Unity.Netcode;
using UnityEngine;

public abstract class AWeapon : NetworkBehaviour
{
    public abstract void Fire();
}
