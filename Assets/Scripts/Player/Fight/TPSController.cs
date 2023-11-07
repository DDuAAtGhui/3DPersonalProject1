using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class TPSController : MonoBehaviour
{
    Player player;
    RigBuilder rigBuilder;

    #region �׼�
    public static Action shootInput;
    public static Action reloadInput;
    #endregion

    [Header("Aim Info")]
    [SerializeField] CinemachineVirtualCamera aimVirtualCamera; //cameraRoot Follow �Ŵ���
    [SerializeField] public GameObject cameraRoot;
    [SerializeField] float AimSensitive = 1f;
    [SerializeField] float Limit_LookVerticalLimit = 70f;
    [SerializeField] GameObject crossHair;
    [SerializeField] GameObject bullet;
    // �̰� �ִϸ��̼� ���� Ÿ������������ �����Ŵϱ� ������Ű�� ����
    [SerializeField] GameObject LaserPoint;
    [SerializeField] bool ActivateLaserPoint;
    [SerializeField] Transform fireTransform;
    bool rayHitFound;


    private void Awake()
    {
        player = GetComponent<Player>();
        rigBuilder = GetComponentInChildren<RigBuilder>();

        LaserPoint.SetActive(false);
    }

    int playerAimingAnimLayerIndex;
    private void Start()
    {
        playerAimingAnimLayerIndex = player.anim.GetLayerIndex("Aiming");

    }

    private void Update()
    {
        #region Ȱ��ȭ�� ������ä�� �÷��̾� �����ϱ�
        //player._InputAim = true;
        //aimVirtualCamera.gameObject.SetActive(true);
        //player.isAiming = true;
        //#region �ִϸ��̼�
        //player.anim.SetLayerWeight(playerAimingAnimLayerIndex,
        //    Mathf.Lerp(player.anim.GetLayerWeight(playerAimingAnimLayerIndex),
        //    1f, Time.deltaTime * 16f));



        //#endregion

        //return;
        #endregion

        AimInfo();
        AimFeatures();

        if (player._InputFire && player.isAiming)
            shootInput?.Invoke();

        if (player._InputReload)
            reloadInput?.Invoke();
    }


    //������ ����ĳ��Ʈ
    Ray ray;
    RaycastHit raycastHit;
    [SerializeField] LayerMask AimRayLayer;
    [HideInInspector] public Vector3 mouseWorldPosition = Vector3.zero;
    private void AimInfo()
    {
        if (!ActivateLaserPoint)
            LaserPoint.SetActive(false);

        mouseWorldPosition = Vector3.zero;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        rayHitFound = Physics.Raycast(ray, out raycastHit, 10000f, AimRayLayer);

        if (rayHitFound)
        {
            mouseWorldPosition = raycastHit.point;

            if (ActivateLaserPoint)
            {
                LaserPoint.SetActive(true);
                LaserPoint.transform.position = mouseWorldPosition;
            }
        }
    }

    bool initialRotation = true;
    private void AimFeatures()
    {
        if (player._InputAim)
        {
            #region �ִϸ��̼�
            player.anim.SetLayerWeight(playerAimingAnimLayerIndex,
                Mathf.Lerp(player.anim.GetLayerWeight(playerAimingAnimLayerIndex),
                1f, Time.deltaTime * 16f));



            #endregion

            aimVirtualCamera.gameObject.SetActive(true);
            crossHair?.SetActive(true);
            rigBuilder.enabled = true;
            player.isAiming = true;

            //�ʱ� ȸ�� ����
            if (initialRotation)
            {
                //�÷��̾� �����̼ǰ� �κ������� �����ϱ����� ���
                Vector3 eulerAngles = player.transform.eulerAngles;

                eulerAngles.x = Quaternion.LookRotation(ray.direction, Vector3.up).eulerAngles.x;

                //����ó�� ȭ���� �÷��̾� �տ��� �÷��̾ ���߰� �־(��, ĳ���� ���� ���ϰ��ִµ�
                //������ ĳ������ �ڸ� �����ϰ� ������) �� ���� ������ �ٷ� �ٶ󺸵��� ����
                eulerAngles.y = Quaternion.LookRotation(ray.direction, Vector3.up).eulerAngles.y;
                player.transform.rotation = Quaternion.Euler(new Vector3(0, eulerAngles.y, 0));
                cameraRoot.transform.Rotate(new Vector3(eulerAngles.x, 0, 0));
                initialRotation = false;
            }



            if (player.Look == Vector2.zero)
                return;

            //Rotate �޼ҵ�ó�� ���� �������ٰ� �߰��� ���ϰ� �������
            float X_Rotation = cameraRoot.transform.eulerAngles.x + player.Look.y * AimSensitive;

            //��������
            //������ ������ XRotation��ġ���� �޶���
            //ī�޶� ���� 0��, ���ݸ� �÷��� 350��, ���ݸ� ������ 10���� ����
            if (X_Rotation >= 65f && X_Rotation <= 300f)
            {
                if (X_Rotation < 180f)
                    X_Rotation = 65f;

                else
                    X_Rotation = 300f;
            }

            cameraRoot.transform.eulerAngles = new Vector3(X_Rotation, cameraRoot.transform.eulerAngles.y, 0);

            //rotation�� ���ο� ������ �ʱ�ȭ �ϴ°Ŷ� Rotate�� ����
            //�÷��̾ ȸ���ϸ� �ڽ��� CameraRoot�� ȸ��0
            player.transform.Rotate(0, player.Look.x * AimSensitive, 0);
        }

        else
        {
            #region �ִϸ��̼�
            player.anim.SetLayerWeight(playerAimingAnimLayerIndex,
                Mathf.Lerp(player.anim.GetLayerWeight(playerAimingAnimLayerIndex),
                0f, Time.deltaTime * 12f));
            #endregion

            aimVirtualCamera.gameObject.SetActive(false);
            crossHair?.SetActive(false);
            rigBuilder.enabled = false; //�ƴ� ���׺��� Ȱ��ȭ�Ǹ� ����� ��Ʋ���־� idle�����϶�
            LaserPoint.SetActive(false);

            player.isAiming = false;
            initialRotation = true;

            cameraRoot.transform.rotation = player.transform.rotation;
        }
    }

    private void OnDrawGizmos()
    {

        //Gizmos.DrawRay(ray);

        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(raycastHit.point, 0.1f);
    }
}
