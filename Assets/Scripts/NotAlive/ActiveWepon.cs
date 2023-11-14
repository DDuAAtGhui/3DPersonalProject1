using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

using UnityEditor.Animations;
using UnityEngine.InputSystem.XR.Haptics;

public class ActiveWepon : MonoBehaviour
{
    public Transform crossHairTarget;
    public Rig handIK; //HandIK 레이어 넣어주기
    RigBuilder rigBuilder;
    GunTweak weapon;

    public Transform weaponParent; // 무기가 위치할 weaponPivot 을 넣어주기

    //에디터.애니메이션으로 녹화
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

        //애니메이션 오버라이드 설정
        animOverride =
            new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animOverride; //필수


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

            //모델이 이상해서 그런지 리그빌더 킨상태면 이상해짐
            rigBuilder.enabled = false;

            anim.SetLayerWeight(animatorLayer_WeaponLayer, 0f);
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

        GameManager.instance.player.isArmed = true;
        rigBuilder.enabled = true;
        handIK.weight = 1f;
        anim.SetLayerWeight(animatorLayer_WeaponLayer, 1f);


        //중복 실행으로 버그터지는거 방지
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

        //현재 장착중인 무기 타입 설정함
        GameManager.instance.player.anim.SetFloat(GameManager.instance.animIDWeaponType,
((float)GetComponentInChildren<GunTweak>().gunData.type));
    }


    [ContextMenu("Save weapon Pose")]
    void SaveWeaponPose()
    {
        //플레이어 녹화
        GameObjectRecorder recorder = new GameObjectRecorder(gameObject);

        //노드들 바인딩 추가. true면 자식까지 바인딩 추가됨
        recorder.BindComponentsOfType<Transform>(weaponParent.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponHintL.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponHintR.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponLeftGrip.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponRightGrip.gameObject, false);
        recorder.TakeSnapshot(0.0f); //0일시 1프레임만 촬영

        recorder.SaveToClip(weapon.weaponAnimation);

    }
}
