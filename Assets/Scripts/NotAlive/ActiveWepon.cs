using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActiveWepon : MonoBehaviour
{
    public Transform crossHairTarget;
    public Rig handIK; //HandIK ���̾� �־��ֱ�
    RigBuilder rigBuilder;
    GunTweak weapon;

    public Transform weaponParent; // ���Ⱑ ��ġ�� weaponPivot �� �־��ֱ�

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

            //���� �̻��ؼ� �׷��� ���׺��� Ų���¸� �̻�����
            rigBuilder.enabled = false;
        }
    }

    public void Equip(GunTweak newWeapon)
    {
        //���� �������Ͻ� �ı�
        if (weapon)
            Destroy(weapon.gameObject);

        weapon = newWeapon;
        weapon.raycastDestination = crossHairTarget;

        weapon.transform.parent = weaponParent;
        weapon.transform.localPosition = weapon.gunData.holdingPosition;
        weapon.transform.localRotation = Quaternion.identity;
    }
}
