using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class TPSController : MonoBehaviour
{
    Player player;

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

        LaserPoint.SetActive(false);
    }

    int playerAimingAnimLayerIndex;
    private void Start()
    {
        playerAimingAnimLayerIndex = player.anim.GetLayerIndex("Aiming");

        initialSpineRotation =
            player.anim.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles;
    }

    private void Update()
    {
        player._InputAim = true;
        aimVirtualCamera.gameObject.SetActive(true);
        player.isAiming = true;
        #region �ִϸ��̼�
        player.anim.SetLayerWeight(playerAimingAnimLayerIndex,
            Mathf.Lerp(player.anim.GetLayerWeight(playerAimingAnimLayerIndex),
            1f, Time.deltaTime * 16f));



        #endregion

        return;

        AimInfo();
        AimFeatures();
    }


    //������ ����ĳ��Ʈ
    Ray ray;
    RaycastHit raycastHit;
    [SerializeField] LayerMask AimRayLayer;
    [HideInInspector] public Vector3 mouseWorldPosition = Vector3.zero;
    [SerializeField] float shootingSpeed = 0.05f;
    [SerializeField] float MOA = 1f;
    float timer = 0f;
    private void AimInfo()
    {
        Vector3 bulletSpread =
            new Vector3(Random.Range(-MOA, MOA), Random.Range(-MOA, MOA), 0);

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

        timer += Time.deltaTime;

        if (player._InputFire)
        {
            Vector3 aimDir = (mouseWorldPosition - fireTransform.position).normalized;

            if (timer >= shootingSpeed)
            {
                timer = 0f;
                GameObject go = Instantiate(bullet, fireTransform.position, Quaternion.LookRotation(aimDir + bulletSpread, Vector3.up));
            }

        }
    }

    bool initialRotation = true;
    Vector3 initialSpineRotation;
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
