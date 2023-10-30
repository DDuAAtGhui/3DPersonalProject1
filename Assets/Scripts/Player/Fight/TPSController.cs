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


    //������ ����ĳ��Ʈ
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
