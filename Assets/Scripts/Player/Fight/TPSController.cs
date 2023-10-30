using Cinemachine;
using UnityEngine;

public class TPSController : MonoBehaviour
{
    Player player;

    [Header("Aim Info")]
    [SerializeField] CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] GameObject cameraRoot;
    [SerializeField] float AimSensitive = 1f;
    [SerializeField] float Limit_LookVerticalLimit = 70f;
    [SerializeField] GameObject crossHair;


    bool rayHitFound;
    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        Aim();
        Rotate();
    }


    //조준점 레이캐스트
    Ray ray;
    RaycastHit raycastHit;
    [SerializeField] LayerMask AimRayLayer;
    private void Aim()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        rayHitFound = Physics.Raycast(ray, out raycastHit, 999f, AimRayLayer);
    }

    private void Rotate()
    {
        if (player._InputAim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            crossHair?.SetActive(true);

            player.isAiming = true;


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
            player.transform.Rotate(0, player.Look.x * AimSensitive, 0);
        }

        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            crossHair?.SetActive(false);
            player.isAiming = false;

            cameraRoot.transform.rotation = player.transform.rotation;
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.DrawRay(ray);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(raycastHit.point, 0.1f);
    }
}
