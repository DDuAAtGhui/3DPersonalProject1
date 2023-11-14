using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

using UnityEditor.Animations;
using UnityEngine.InputSystem.XR.Haptics;

public class ActiveWepon : MonoBehaviour
{
    public Transform crossHairTarget;
    public Rig handIK; //HandIK ���̾� �־��ֱ�
    RigBuilder rigBuilder;
    GunTweak weapon;

    public Transform weaponParent; // ���Ⱑ ��ġ�� weaponPivot �� �־��ֱ�

    //������.�ִϸ��̼����� ��ȭ
    public Transform weaponLeftGrip;
    public Transform weaponRightGrip;
    public Transform weaponHintL;
    public Transform weaponHintR;

    Animator anim;

    public AnimatorOverrideController animOverride;
    int animatorLayer_WeaponLayer;

    void Start()
    {
        GunTweak currentWeapon = GetComponentInChildren<GunTweak>();
        rigBuilder = GetComponent<RigBuilder>();
        anim = GetComponent<Animator>();

        //�ִϸ��̼� �������̵� ����
        animOverride =
            new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animOverride; //�ʼ�


        animatorLayer_WeaponLayer = anim.GetLayerIndex("Weapon Layer");

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
            handIK.weight = 0f;

            GameManager.instance.player.isArmed = false;

            //���� �̻��ؼ� �׷��� ���׺��� Ų���¸� �̻�����
            rigBuilder.enabled = false;

            anim.SetLayerWeight(animatorLayer_WeaponLayer, 0f);
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

        GameManager.instance.player.isArmed = true;
        rigBuilder.enabled = true;
        handIK.weight = 1f;
        anim.SetLayerWeight(animatorLayer_WeaponLayer, 1f);


        //�ߺ� �������� ���������°� ����
        Invoke(nameof(SetAnimationDelayed), 0.001f);
    }
    void SetAnimationDelayed()
    {
        if (animOverride != null)
        {
            Debug.Log("Before : " + animOverride["weapon_anim_Empty"]);
            animOverride["weapon_anim_Empty"] = weapon.weaponAnimation;
            Debug.Log("Current : " + animOverride["weapon_anim_Empty"]);

            // anim.Update(0f);
        }
        else
            Debug.Log("animOverride is NULL");

        //���� �������� ���� Ÿ�� ������
        GameManager.instance.player.anim.SetFloat(GameManager.instance.animIDWeaponType,
((float)GetComponentInChildren<GunTweak>().gunData.type));
    }


    [ContextMenu("Save weapon Pose")]
    void SaveWeaponPose()
    {
        //�÷��̾� ��ȭ
        GameObjectRecorder recorder = new GameObjectRecorder(gameObject);

        //���� ���ε� �߰�. true�� �ڽı��� ���ε� �߰���
        recorder.BindComponentsOfType<Transform>(weaponParent.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponHintL.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponHintR.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponLeftGrip.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponRightGrip.gameObject, false);
        recorder.TakeSnapshot(0.0f); //0�Ͻ� 1�����Ӹ� �Կ�

        recorder.SaveToClip(weapon.weaponAnimation);

    }
}
