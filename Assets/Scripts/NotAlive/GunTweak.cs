using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GunTweak : MonoBehaviour
{
    [SerializeField] public Gundata gunData;
    [SerializeField] Transform fireTransform;
    public Transform raycastDestination;
    TPSController tpsController;

    GameObject muzzleFireObject;
    ParticleSystem[] muzzleFires;

    int currentMaxAmmo;

    float timePassSinceLastShooting;
    float SecondPerRound;
    //1초 / rps = 1발 발사당 소요되는 시간
    bool CanShoot() => !gunData.isReloading &&
        timePassSinceLastShooting > SecondPerRound;
    bool isOwnerPlayer => GetComponentInParent<Player>();
    Player player;
    RigBuilder playerRigbuilder;
    public AnimationClip weaponAnimation;

    AudioSource audioSource;

    [SerializeField] TextMeshProUGUI currentAmmoTxt;
    [SerializeField] TextMeshProUGUI totalAmmoTxt;

    public Transform clipSlot; //인스펙터에서 넣기
    public GameObject MagClip; //인스펙터에서 넣기
    GameObject instance_mag;
    private void Start()
    {
        //스크립터블 오브젝트는 씬 끝나도 데이터 저장되어있으므로
        //bool값 초기화
        gunData.isReloading = false;
        gunData.currentAmmo = gunData.magSize;
        currentMaxAmmo = gunData.MaxAmmo;

        tpsController = GetComponentInParent<TPSController>();
        audioSource = GetComponent<AudioSource>();

        Rigidbody MagRb;

        if (MagClip.GetComponent<Rigidbody>() == null)
            MagClip.AddComponent<Rigidbody>();

        MagRb = MagClip.GetComponent<Rigidbody>();
        MagRb.mass = 0.5f;
        MagRb.useGravity = false;
        MagRb.GetComponent<BoxCollider>().enabled = false;

        if (isOwnerPlayer) //총이 플레이어 자식으로 존재하면
        {
            player = GetComponentInParent<Player>();
            playerRigbuilder = player.GetComponent<RigBuilder>();
            currentAmmoTxt = GameObject.Find("CurrentAmmo").GetComponent<TextMeshProUGUI>();
            totalAmmoTxt = GameObject.Find("TotalAmmo").GetComponent<TextMeshProUGUI>();
            currentAmmoTxt.text = gunData.currentAmmo.ToString();
            totalAmmoTxt.text = gunData.MaxAmmo.ToString();


            //델리게이트 등록
            TPSController.shootInput += Shoot;
            TPSController.reloadInput += DoReload;
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
                currentAmmoTxt.text = gunData.currentAmmo.ToString();

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

        if (gunData.fireSound != null)
        {
            audioSource.PlayOneShot(gunData.fireSound);
        }
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
        if ((!gunData.isReloading && gunData.currentAmmo != gunData.magSize) || tpsController.aimToggleDebug)
            StartCoroutine(Reload());

        Debug.Log("Debug : currentTotalAmmo :" + currentMaxAmmo);
    }

    IEnumerator Reload()
    {
        gunData.isReloading = true;
        player.isReloading = true;
        MagClip.SetActive(false);

        instance_mag = Instantiate(MagClip, clipSlot.transform.position,
            clipSlot.transform.rotation);

        instance_mag.transform.localScale = clipSlot.transform.localScale;

        instance_mag.SetActive(true);
        instance_mag.AddComponent<MagController>();


        Rigidbody instance_magRB = instance_mag.GetComponent<Rigidbody>();

        if (clipSlot != null && MagClip != null)
        {
            instance_mag.GetComponent<BoxCollider>().enabled = true;
            instance_magRB.useGravity = true;
            instance_magRB.AddForce(transform.InverseTransformDirection(Vector3.down)
                , ForceMode.Impulse);
        }

        switch (isOwnerPlayer)
        {
            case true:
                player.anim.Play(gunData.reloadAnimation.name);
                currentMaxAmmo -= gunData.magSize - gunData.currentAmmo;
                gunData.currentAmmo = gunData.magSize;
                currentAmmoTxt.text = gunData.currentAmmo.ToString();
                totalAmmoTxt.text = currentMaxAmmo.ToString();
                break;
            case false:
                break;
        }

        //스크립터블로 설정한 장전시간동안 isReolading이 true
        yield return new WaitForSeconds(gunData.reloadTime);
        gunData.isReloading = false;
        player.isReloading = false;

        if (clipSlot != null)
        {
            MagClip.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        //현재 스크립트 파괴시 델리게이트 구독 해제
        TPSController.shootInput -= Shoot;
        TPSController.reloadInput -= DoReload;
    }
}
