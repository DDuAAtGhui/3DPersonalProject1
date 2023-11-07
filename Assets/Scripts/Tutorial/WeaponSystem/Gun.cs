using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun : MonoBehaviour
{
    [SerializeField] Gundata gunData;
    [SerializeField] Transform muzzle;

    float timePassSinceLastShot;

    private void Start()
    {
        // 델리게이트 등록
        PlayerShoot.shootInput += Shoot;
        PlayerShoot.reloadInput += DoReload;
    }

    // 만약 발사속도가 600rpm이라 가정할 때
    // 1분에 600발이므로 600rpm / 1분 = 1초당 발사속도
    // 즉 600rpm / 60s = 10rps = 1초당 10발 발사
    // 1초 / 10rps = 0.1 초당 1발
    bool CanShoot() => !gunData.isReloading &&
        timePassSinceLastShot > 1f / (gunData.fireRate / 60f); // 1초/rps

    public void DoReload()
    {
        if (!gunData.isReloading)
        {
            Debug.Log("Reload Check");
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        gunData.isReloading = true;

        yield return new WaitForSeconds(gunData.reloadTime);

        gunData.currentAmmo = gunData.magSize;
        gunData.isReloading = false;
    }
    public void Shoot()
    {
        if (gunData.currentAmmo > 0)
        {
            if (CanShoot())
            {
                if (Physics.Raycast(muzzle.position, transform.forward,
                    out RaycastHit hitInfo, gunData.maxDistance))
                {
                    IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                    damageable?.TakeDamage(gunData.damage);
                }

                gunData.currentAmmo--;
                timePassSinceLastShot = 0;
                OnGunShot();
            }
        }
    }

    private void Update()
    {
        timePassSinceLastShot += Time.deltaTime;

        Debug.DrawRay(muzzle.position, muzzle.forward, Color.green);
    }

    private void OnGunShot()
    {

    }
}
