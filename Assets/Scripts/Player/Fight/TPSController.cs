using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

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
    [SerializeField] Transform fireTransform;

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


        if (player._InputFire)
        {
            Vector3 aimDir = (mouseWorldPosition - fireTransform.position).normalized;
            GameObject go = Instantiate(bullet, fireTransform.position, Quaternion.LookRotation(aimDir, Vector3.up));
        }
    }


    //������ ����ĳ��Ʈ
    Ray ray;
    RaycastHit raycastHit;
    [SerializeField] LayerMask AimRayLayer;
    [HideInInspector] public Vector3 mouseWorldPosition;
    private void Aim()
    {
        mouseWorldPosition = Vector3.zero;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        rayHitFound = Physics.Raycast(ray, out raycastHit, 999f, AimRayLayer);

        if (rayHitFound)
        {
            mouseWorldPosition = raycastHit.point;
        }

    }

    bool initialRotation = true;
    private void Rotate()
    {
        if (player._InputAim)
        {
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
            aimVirtualCamera.gameObject.SetActive(false);
            crossHair?.SetActive(false);
            player.isAiming = false;
            initialRotation = true;

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
