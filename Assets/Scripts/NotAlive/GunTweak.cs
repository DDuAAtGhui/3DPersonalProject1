using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GunTweak : MonoBehaviour
{
    [SerializeField] TPSController tpsController;
    [SerializeField] public Gundata gunData;
    [SerializeField] Transform fireTransform;

    GameObject muzzleFireObject;
    ParticleSystem[] muzzleFires;

    float timePassSinceLastShooting;
    float SecondPerRound;
    //1�� / rps = 1�� �߻�� �ҿ�Ǵ� �ð�
    bool CanShoot() => !gunData.isReloading &&
        timePassSinceLastShooting > SecondPerRound;
    bool isOwnerPlayer => GetComponentInParent<Player>() != null;
    Player player;
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

            player = GetComponentInParent<Player>();
        }

        #region Initialize MuzzleFire
        //������Ʈ �����ϰ� ���� �ڽ����� ��ġ�ϰ� ����
        muzzleFireObject = Instantiate(gunData.muzzleFire,
            fireTransform.position, Quaternion.identity, fireTransform);
        muzzleFireObject.SetActive(true);


        //�������� ��ƼŬ �ý��� ������Ʈ ������
        muzzleFires = muzzleFireObject.GetComponentsInChildren<ParticleSystem>();

        foreach (var muzzleFire in muzzleFires)
            muzzleFire.Stop();
        #endregion
    }

    private void Update()
    {
        SecondPerRound = 1f / (gunData.fireRate / 60f);

        timePassSinceLastShooting += Time.deltaTime;

        muzzleFireObject.transform.rotation =
    Quaternion.LookRotation(fireTransform.forward, Vector3.up);
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

        // StartCoroutine(MuzzleFire());

        foreach (var muzzleFire in muzzleFires)
            muzzleFire.Emit(1);

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

    IEnumerator MuzzleFire()
    {
        //if (muzzleFireObject.activeSelf)
        //    yield break;

        muzzleFireObject.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        muzzleFireObject.SetActive(false);
    }

    private void DoReload()
    {
        if (!gunData.isReloading)
            StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        gunData.isReloading = true;

        switch (isOwnerPlayer)
        {
            case true:

                player.anim.Play(gunData.reloadAnimation.name);
                break;
            case false:
                break;
        }

        //��ũ���ͺ�� ������ �����ð����� isReolading�� true
        yield return new WaitForSeconds(gunData.reloadTime);

        gunData.currentAmmo = gunData.magSize;
        gunData.isReloading = false;
    }
}
