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
    //1초 / rps = 1발 발사당 소요되는 시간
    bool CanShoot() => !gunData.isReloading &&
        timePassSinceLastShooting > SecondPerRound;
    bool isOwnerPlayer => GetComponentInParent<Player>() != null;
    Player player;
    private void Start()
    {
        //스크립터블 오브젝트는 씬 끝나도 데이터 저장되어있으므로
        //bool값 초기화
        gunData.isReloading = false;
        gunData.currentAmmo = gunData.magSize;

        if (isOwnerPlayer) //총이 플레이어 자식으로 존재하면
        {
            //델리게이트 등록
            TPSController.shootInput += Shoot;
            TPSController.reloadInput += DoReload;

            player = GetComponentInParent<Player>();
        }

        #region Initialize MuzzleFire
        //오브젝트 생성하고 총의 자식으로 배치하고 꺼둠
        muzzleFireObject = Instantiate(gunData.muzzleFire,
            fireTransform.position, Quaternion.identity, fireTransform);
        muzzleFireObject.SetActive(true);


        //프리팹의 파티클 시스템 컴포넌트 가져옴
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

        //탄 퍼짐
        Vector3 bulletSpread = new Vector3(UnityEngine.Random.Range(-gunData.bulletSpread, gunData.bulletSpread),
            UnityEngine.Random.Range(-gunData.bulletSpread, gunData.bulletSpread), 0);

        //총알 생성
        GameObject go = Instantiate(gunData.bullet, fireTransform.position
            , Quaternion.LookRotation(aimDir + bulletSpread, Vector3.up)
            , transform); //나올때 총알이 자식으로 나오게해서
                          //나온 총의 스크립터블 데이터값에 접근할거임

        go.AddComponent<RememberingParent>(); //부모 오브젝트 기억

        go.transform.SetParent(null); //부모화 해제
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

        //스크립터블로 설정한 장전시간동안 isReolading이 true
        yield return new WaitForSeconds(gunData.reloadTime);

        gunData.currentAmmo = gunData.magSize;
        gunData.isReloading = false;
    }
}
