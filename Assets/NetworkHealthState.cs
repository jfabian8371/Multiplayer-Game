using Unity.Netcode;
using UnityEngine;

public class NetworkHealthState : NetworkBehaviour
{
    [HideInInspector]
    public NetworkVariable<int> HealthPoint = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        //base.OnNetworkSpawn();
        //HealthPoint.Value = 100;
        if (IsServer){
            HealthPoint.Value = 100;
        }
    }

    // void OnTriggerEnter(Collider collider)
    // {
    //     if(!IsServer){
    //         return;
    //     }
    //     if(collider.GetComponent<Bullet>()){
    //         GetComponent<NetworkHealthState>().HealthPoint.Value -= 10;
    //     }
    // }
}
