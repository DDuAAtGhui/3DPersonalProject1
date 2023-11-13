using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActiveWepon : MonoBehaviour
{
    public Transform crossHairTarget;
    public Rig handIK; //HandIK 레이어 넣어주기
    RigBuilder rigBuilder;
    GunTweak weapon;

    public Transform weaponParent; // 무기가 위치할 weaponPivot 을 넣어주기

    void Start()
    {
        GunTweak currentWeapon = GetComponentInChildren<GunTweak>();
        rigBuilder = GetComponent<RigBuilder>();
        if (currentWeapon)
        {
            Equip(currentWeapon);
        }
    }

    void Update()
    {
        if (weapon)
        {
            handIK.weight = 1f;

            GameManager.instance.player.isArmed = true;

            rigBuilder.enabled = true;
        }

        else
        {
            handIK.weight = 0f;

            GameManager.instance.player.isArmed = false;

            //모델이 이상해서 그런지 리그빌더 킨상태면 이상해짐
            rigBuilder.enabled = false;
        }
    }

    public void Equip(GunTweak newWeapon)
    {
        //무기 장착중일시 파괴
        if (weapon)
            Destroy(weapon.gameObject);

        weapon = newWeapon;
        weapon.raycastDestination = crossHairTarget;

        weapon.transform.parent = weaponParent;
        weapon.transform.localPosition = weapon.gunData.holdingPosition;
        weapon.transform.localRotation = Quaternion.identity;
    }
}
