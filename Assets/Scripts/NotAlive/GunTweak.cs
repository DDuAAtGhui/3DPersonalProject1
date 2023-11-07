using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTweak : MonoBehaviour
{
    [SerializeField] TPSController tpsController;
    [SerializeField] public Gundata gunData;
    [SerializeField] Transform fireTransform;

    float timePassSinceLastShooting;

    //1�� / rps = 1�� �߻�� �ҿ�Ǵ� �ð�
    bool CanShoot() => !gunData.isReloading &&
        timePassSinceLastShooting > 1f / (gunData.fireRate / 60f);
    bool isOwnerPlayer => GetComponentInParent<Player>() != null;
    private void Start()
    {
        //��ũ���ͺ� ������Ʈ�� �� ������ ������ ����Ǿ������Ƿ�
        //bool�� �ʱ�ȭ
        gunData.isReloading = false;
        gunData.currentAmmo = gunData.magSize;

        if (isOwnerPlayer) //���� �÷��̾� �ڽ����� �����ϸ�
        {
            //��������Ʈ ���
            TPSController.shootInput += Shoot;
            TPSController.reloadInput += DoReload;
        }
    }

    private void Update()
    {
        timePassSinceLastShooting += Time.deltaTime;
    }

    Vector3 aimDir;
    private void Shoot()
    {
        if (gunData.currentAmmo > 0)
        {
            if (CanShoot())
            {
                gunData.currentAmmo--;
                timePassSinceLastShooting = 0;
                OnGunShot();
            }
        }
    }

    private void OnGunShot()
    {
        switch (isOwnerPlayer)
        {
            case true:
                aimDir = (tpsController.mouseWorldPosition -
                    fireTransform.position).normalized;

                break;
            case false:
                break;
        }

        //ź ����
        Vector3 bulletSpread = new Vector3(UnityEngine.Random.Range(-gunData.bulletSpread, gunData.bulletSpread),
            UnityEngine.Random.Range(-gunData.bulletSpread, gunData.bulletSpread), 0);

        //�Ѿ� ����
        GameObject go = Instantiate(gunData.bullet, fireTransform.position
            , Quaternion.LookRotation(aimDir + bulletSpread, Vector3.up)
            , transform); //���ö� �Ѿ��� �ڽ����� �������ؼ�
                          //���� ���� ��ũ���ͺ� �����Ͱ��� �����Ұ���

        go.AddComponent<RememberingParent>(); //�θ� ������Ʈ ���

        go.transform.SetParent(null); //�θ�ȭ ����
    }

    private void DoReload()
    {
        if (!gunData.isReloading)
            StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        gunData.isReloading = true;

        //��ũ���ͺ�� ������ �����ð����� isReolading�� true
        yield return new WaitForSeconds(gunData.reloadTime);

        gunData.currentAmmo = gunData.magSize;
        gunData.isReloading = false;
    }
}
