using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerWeapon : NetworkBehaviour
{
    [SerializeField] private List<AWeapon> weapons = new List<AWeapon>();

    [SerializeField] private AWeapon currentWeapon;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            FireWeapon();
        }
    }
    public void InitalizeWeapons(Transform parentOfWeapon){
        for (int i = 0; i < weapons.Count; i++){
            weapons[i].transform.SetParent(parentOfWeapon);
        }
        InitializeWeapon(0);
    }

    private void InitializeWeapon(int weaponIndex){
        for(int i = 0; i < weapons.Count; i++){
            weapons[i].gameObject.SetActive(false);
        }
        if(weapons.Count > weaponIndex){
            currentWeapon = weapons[weaponIndex];
        }
    }
    private void FireWeapon(){
        currentWeapon.Fire();
    }
}
