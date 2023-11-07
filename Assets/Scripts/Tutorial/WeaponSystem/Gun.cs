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
        // ��������Ʈ ���
        PlayerShoot.shootInput += Shoot;
        PlayerShoot.reloadInput += DoReload;
    }

    // ���� �߻�ӵ��� 600rpm�̶� ������ ��
    // 1�п� 600���̹Ƿ� 600rpm / 1�� = 1�ʴ� �߻�ӵ�
    // �� 600rpm / 60s = 10rps = 1�ʴ� 10�� �߻�
    // 1�� / 10rps = 0.1 �ʴ� 1��
    bool CanShoot() => !gunData.isReloading &&
        timePassSinceLastShot > 1f / (gunData.fireRate / 60f); // 1��/rps

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
