using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    public GunTweak weaponFab;
    private void OnTriggerEnter(Collider other)
    {
        ActiveWepon activeWeapon = other.gameObject.GetComponent<ActiveWepon>();

        if (activeWeapon)
        {
            GunTweak newWeapon = Instantiate(weaponFab);
            newWeapon.enabled = true;
            activeWeapon.Equip(newWeapon);
        }
    }
}
