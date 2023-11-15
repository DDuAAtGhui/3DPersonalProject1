using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActiveWepon : MonoBehaviour
{
    Player player;

    public Transform crossHairTarget;
    public Rig handIK; //HandIK ���̾� �־��ֱ�
    RigBuilder rigBuilder;
    GunTweak weapon;

    public Transform weaponParent; // ���Ⱑ ��ġ�� weaponPivot �� �־��ֱ�

    public Animator rigController;
    int animatorLayer_WeaponLayer;

    void Start()
    {
        player = GetComponent<Player>();
        GunTweak currentWeapon = GetComponentInChildren<GunTweak>();
        rigBuilder = GetComponent<RigBuilder>();

        animatorLayer_WeaponLayer = rigController.GetLayerIndex("Weapon Layer");

        if (currentWeapon)
        {
            Equip(currentWeapon);
        }
    }

    void Update()
    {

        if (weapon)
        {
            // Debug.Log("currentWeapon : " + GetComponentInChildren<GunTweak>().name);
        }
        else
        {
            player.isArmed = false;
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
        rigController.Play("Equip_" + weapon.gunData.name);

        player.isArmed = true;
        rigBuilder.enabled = true;
        handIK.weight = 1f;
        player.anim.SetFloat(GameManager.instance.animIDWeaponType,
            (float)weapon.gunData.type);
    }

    [Header("Weapon Holster")]
    public Transform holster_Back;
    public Transform holster_RightThigh;
    public void HolsterTweak()
    {
        if (weapon == null)
            return;

        if (player.isAiming)
        {
            weapon.transform.parent = weaponParent;
            weapon.transform.localPosition = weapon.gunData.holdingPosition;
            weapon.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Transform tempParent;
            switch (weapon.gunData.type)
            {
                case Type.Pistol:
                    tempParent = holster_RightThigh.transform;
                    break;
                default:
                    tempParent = holster_Back.transform;
                    break;
            }
            weapon.transform.parent = tempParent;
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
        }
    }
}
