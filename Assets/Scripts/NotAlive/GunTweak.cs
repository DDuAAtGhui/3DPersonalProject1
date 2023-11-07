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

    //1초 / rps = 1발 발사당 소요되는 시간
    bool CanShoot() => !gunData.isReloading &&
        timePassSinceLastShooting > 1f / (gunData.fireRate / 60f);
    bool isOwnerPlayer => GetComponentInParent<Player>() != null;
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

    private void DoReload()
    {
        if (!gunData.isReloading)
            StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        gunData.isReloading = true;

        //스크립터블로 설정한 장전시간동안 isReolading이 true
        yield return new WaitForSeconds(gunData.reloadTime);

        gunData.currentAmmo = gunData.magSize;
        gunData.isReloading = false;
    }
}
