using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    //1�� / rps = 1�� �߻�� �ҿ�Ǵ� �ð�
    bool CanShoot() => !gunData.isReloading &&
        timePassSinceLastShooting > SecondPerRound;
    bool isOwnerPlayer => GetComponentInParent<Player>();
    Player player;

    public AnimationClip weaponAnimation;

    AudioSource audioSource;

    [SerializeField] TextMeshProUGUI currentAmmoTxt;
    [SerializeField] TextMeshProUGUI totalAmmoTxt;
    private void Start()
    {
        //��ũ���ͺ� ������Ʈ�� �� ������ ������ ����Ǿ������Ƿ�
        //bool�� �ʱ�ȭ
        gunData.isReloading = false;
        gunData.currentAmmo = gunData.magSize;
        currentMaxAmmo = gunData.MaxAmmo;

        tpsController = GetComponentInParent<TPSController>();
        audioSource = GetComponent<AudioSource>();

        if (isOwnerPlayer) //���� �÷��̾� �ڽ����� �����ϸ�
        {
            player = GetComponentInParent<Player>();
            currentAmmoTxt = GameObject.Find("CurrentAmmo").GetComponent<TextMeshProUGUI>();
            totalAmmoTxt = GameObject.Find("TotalAmmo").GetComponent<TextMeshProUGUI>();
            currentAmmoTxt.text = gunData.currentAmmo.ToString();
            totalAmmoTxt.text = gunData.MaxAmmo.ToString();

            //��������Ʈ ���
            TPSController.shootInput += Shoot;
            TPSController.reloadInput += DoReload;
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
                currentAmmoTxt.text = gunData.currentAmmo.ToString();

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
        if (!gunData.isReloading && gunData.currentAmmo != gunData.magSize)
            StartCoroutine(Reload());

        Debug.Log("Debug : currentTotalAmmo :" + currentMaxAmmo);
    }

    IEnumerator Reload()
    {
        gunData.isReloading = true;

        switch (isOwnerPlayer)
        {
            case true:
                player.anim.Play(gunData.reloadAnimation.name);
                currentMaxAmmo -= gunData.magSize - gunData.currentAmmo;
                gunData.currentAmmo = gunData.magSize;
                gunData.isReloading = false;
                currentAmmoTxt.text = gunData.currentAmmo.ToString();
                totalAmmoTxt.text = currentMaxAmmo.ToString();
                break;
            case false:
                break;
        }

        //��ũ���ͺ�� ������ �����ð����� isReolading�� true
        yield return new WaitForSeconds(gunData.reloadTime);

    }

    private void OnDestroy()
    {
        //���� ��ũ��Ʈ �ı��� ��������Ʈ ���� ����
        TPSController.shootInput -= Shoot;
        TPSController.reloadInput -= DoReload;
    }
}
