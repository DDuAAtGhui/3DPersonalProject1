using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class TPSController : MonoBehaviour
{
    Player player;

    [Header("Aim Info")]
    [SerializeField] CinemachineVirtualCamera aimVirtualCamera; //cameraRoot Follow 거는중
    [SerializeField] public GameObject cameraRoot;
    [SerializeField] float AimSensitive = 1f;
    [SerializeField] float Limit_LookVerticalLimit = 70f;
    [SerializeField] GameObject crossHair;
    [SerializeField] GameObject bullet;
    // 이거 애니메이션 리깅 타겟포지션으로 잡을거니까 생성시키지 말기
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
        #region 애니메이션
        player.anim.SetLayerWeight(playerAimingAnimLayerIndex,
            Mathf.Lerp(player.anim.GetLayerWeight(playerAimingAnimLayerIndex),
            1f, Time.deltaTime * 16f));



        #endregion

        return;

        AimInfo();
        AimFeatures();
    }


    //조준점 레이캐스트
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
            #region 애니메이션
            player.anim.SetLayerWeight(playerAimingAnimLayerIndex,
                Mathf.Lerp(player.anim.GetLayerWeight(playerAimingAnimLayerIndex),
                1f, Time.deltaTime * 16f));



            #endregion

            aimVirtualCamera.gameObject.SetActive(true);
            crossHair?.SetActive(true);
            player.isAiming = true;

            //초기 회전 설정
            if (initialRotation)
            {
                //플레이어 로테이션값 부분적으로 유지하기위해 사용
                Vector3 eulerAngles = player.transform.eulerAngles;

                eulerAngles.x = Quaternion.LookRotation(ray.direction, Vector3.up).eulerAngles.x;

                //원신처럼 화면이 플레이어 앞에서 플레이어를 비추고 있어도(즉, 캐릭터 앞을 향하고있는데
                //유저는 캐릭터의 뒤를 조준하고 있을때) 그 조준 방향을 바로 바라보도록 설정
                eulerAngles.y = Quaternion.LookRotation(ray.direction, Vector3.up).eulerAngles.y;
                player.transform.rotation = Quaternion.Euler(new Vector3(0, eulerAngles.y, 0));
                cameraRoot.transform.Rotate(new Vector3(eulerAngles.x, 0, 0));
                initialRotation = false;
            }



            if (player.Look == Vector2.zero)
                return;

            //Rotate 메소드처럼 현재 각도에다가 추가로 더하게 만들어줌
            float X_Rotation = cameraRoot.transform.eulerAngles.x + player.Look.y * AimSensitive;

            //각도제한
            //제한할 각도는 XRotation수치따라 달라짐
            //카메라 정면 0도, 조금만 올려도 350도, 조금만 내리면 10도인 상태
            if (X_Rotation >= 65f && X_Rotation <= 300f)
            {
                if (X_Rotation < 180f)
                    X_Rotation = 65f;

                else
                    X_Rotation = 300f;
            }

            cameraRoot.transform.eulerAngles = new Vector3(X_Rotation, cameraRoot.transform.eulerAngles.y, 0);

            //rotation은 새로운 각도로 초기화 하는거라 Rotate를 써줌
            //플레이어가 회전하면 자식인 CameraRoot도 회전0
            player.transform.Rotate(0, player.Look.x * AimSensitive, 0);
        }

        else
        {
            #region 애니메이션
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
