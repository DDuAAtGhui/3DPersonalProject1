using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "WeaponSystem/Create New Gun")]
public class Gundata : ScriptableObject
{
    [Header("Info")]
    public new string name;

    [Header("Shot Info")]
    public float damage;
    public float maxDistance;
    public int fireRate;
    public GameObject bullet;
    public float bulletSpread = 0.01f;
    public float muzzleFire;

    [Header("Reload Info")]
    public float reloadTime;
    [HideInInspector] public bool isReloading;

    public int currentAmmo;
    public int magSize;

}
