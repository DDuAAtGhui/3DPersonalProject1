using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    GameObject currentWeapon;
    GunTweak currentWeaponGunTweak;
    Player player;
    GameManager gameManager;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        gameManager = GameManager.instance;
        currentWeapon = transform.GetChild(0).gameObject;
        currentWeaponGunTweak = currentWeapon.GetComponent<GunTweak>();
    }

    private void Update()
    {
        player.anim.SetBool(gameManager.animIDisArmed, player.isArmed);
        player.anim.SetFloat(gameManager.animIDWeaponType,
            (float)currentWeaponGunTweak.gunData.type);

        if (!player.isArmed)
            currentWeapon.SetActive(false);

        else
            currentWeapon.SetActive(true);

        //if(player._InputKeyNums.Contains(true))
        //    player.isArmed = true;


    }
}
