using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    None,AssultRifle, Rifle, Pistol
}

[CreateAssetMenu(fileName = "Gun", menuName = "WeaponSystem/Create New Gun")]
public class Gundata : ScriptableObject
{

    [Header("Info")]
    public new string name;
    public Type type;
    public Vector3 holdingPosition = Vector3.zero;

    [Header("Shot Info")]
    public float damage;
    public float maxDistance;
    public int fireRate;
    public GameObject bullet;
    public float bulletSpread = 0.01f;

    public GameObject muzzleFire;
    //파티클 시스템 단독으로만 존재할경우
    //public ParticleSystem muzzleFire;

    [Header("Reload Info")]
    public AnimationClip reloadAnimation;
    public float reloadTime;
    [HideInInspector] public bool isReloading;


    public int currentAmmo;
    public int magSize;

}
