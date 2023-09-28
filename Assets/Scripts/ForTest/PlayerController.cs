using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region components
    CharacterController CC;
    PlayerInputSystem playerInput;
    Animator anim;
    [SerializeField] CinemachineFreeLook Vcam_FreeLook;

    #endregion
    #region infos
    [Header("Move Info")]
    [SerializeField] Vector3 MoveTarget;
    [SerializeField] float MoveSpeed = 5f;
    [SerializeField] float SpeedMultiplierToWalk = 2.5f;
    [SerializeField] float CurrentSpeed;
    [SerializeField] float targetSpeed;

    [Header("Rotate Info")]
    //1프레임당 회전 감소율
    [SerializeField] float rotationFactorPerFrame = 25f;

    [Header("Input Info")]

    [Header("Camera Info")]
    [SerializeField][Tooltip("마우스 X축 현재속도")] float Mouse_X;
    [SerializeField][Tooltip("마우스 Y축 현재속도")] float Mouse_Y;
    [SerializeField] float Mouse_sensitivity = 1f;
    [SerializeField] Vector3 look;
    [SerializeField] Vector3 cameraForward;

    #endregion

    //current : 월드 좌표계 중심으로 움직임
    Vector2 currentMovementInput;
    Vector3 currentMovement;

    //local : current 기준으로 내 캐릭터 좌표계 중심으로 움직이게 할거임
    Vector3 localMovement;
    bool isMovementPressed;

    Vector3 currentWalkMovement;
    bool isWalkPressed;

    private void Awake()
    {
        CC = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();


        playerInput = new PlayerInputSystem();

        //인스턴스 생성
        //started 콜백에 onMoveAction 메소드 등록
        playerInput.CharacterControls.Move.started += onMoveAction;
        //조이스틱은 스틱으로 컨트롤하니까 perform필수임
        //근데 키보드도 결국 4방향만 할거 아니면 perform하는게 좋음
        //performed은 0과 1사이의 값을 콜백받음
        playerInput.CharacterControls.Move.performed += onMoveAction;
        //키가 떼졌는지를 감지하는 콜백
        playerInput.CharacterControls.Move.canceled += onMoveAction;


        playerInput.CharacterControls.Walk.started += onWalkAction;
        playerInput.CharacterControls.Walk.canceled += onWalkAction;

        playerInput.CharacterControls.Look.performed += onLookAction;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    //Move 액션 입력을 감지하고 started콜백 받아서 context => 이하의 함수를 수행한다

    private void Update()
    {
        handleCamera();

        // WASD 입력 감지
        if (isMovementPressed)
        {
            //cameraForward 기준으로 입력 벡터 y의 값 + 자식 오브젝트가 플레이어를 항상 바라보는데 거기의 오른쪽 벡터에 x축입력값 보정
            currentMovement = Vcam_FreeLook.transform.GetChild(0).transform.right * currentMovementInput.x + cameraForward * currentMovementInput.y;

            currentWalkMovement = currentMovement * SpeedMultiplierToWalk;
        }

        if (isWalkPressed)
            CC.Move(currentWalkMovement * MoveSpeed * Time.deltaTime);


        else
            CC.Move(currentMovement * MoveSpeed * Time.deltaTime);

        handleAnimation();
        handleRotation();
    }

    //인풋시스템 쓸 때 항상 키고 끄고 해줘야함
    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    void onMoveAction(InputAction.CallbackContext context)
    {
        //WASD 입력값 저장
        currentMovementInput = context.ReadValue<Vector2>();

        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;

        //currentMovemnt를 cameraForward 방향으로 정렬(카메라가 플레이어를 바라보는 방향 = 플레이어가 게임에서 보는 방향)
        currentMovement = Quaternion.LookRotation(cameraForward) * currentMovement;


        currentWalkMovement.x = currentMovementInput.x * SpeedMultiplierToWalk;
        currentWalkMovement.z = currentMovementInput.y * SpeedMultiplierToWalk;

        //WASD중에 하나라도 눌렸는지 체크
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;

        //로컬 기준 움직임으로 전환
        localMovement = transform.TransformDirection(new Vector3(currentMovementInput.x, 0, currentMovementInput.y));

    }

    void onWalkAction(InputAction.CallbackContext context)
    {
        isWalkPressed = context.ReadValueAsButton();
    }

    void onLookAction(InputAction.CallbackContext context)
    {
        Mouse_X = context.ReadValue<Vector2>().x * Mouse_sensitivity;
        Mouse_Y = context.ReadValue<Vector2>().y * Mouse_sensitivity;
    }
    void handleAnimation()
    {
        CurrentSpeed = CC.velocity.magnitude;

        anim.SetFloat("Speed", Mathf.Lerp(0, CurrentSpeed, 1));
    }


    void handleRotation()
    {
        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentMovement);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void handleCamera()
    {
        //B - A : A가 B를 바라보는 방향의 벡터값
        //A - B : B가 A를 바라보는 방향의 벡터값
        look = (transform.position - Vcam_FreeLook.transform.position).normalized;

        //y축 플레이어랑 동일하게해서 2차원 평면처럼 움직일 수 있게 방향 벡터값 구함
        cameraForward = (transform.position - new Vector3(Vcam_FreeLook.transform.position.x, transform.position.y, Vcam_FreeLook.transform.position.z)).normalized;

        Debug.DrawLine(Vcam_FreeLook.transform.position, transform.position, Color.red); // look
        Debug.DrawLine(new Vector3(Vcam_FreeLook.transform.position.x, transform.position.y, Vcam_FreeLook.transform.position.z), transform.position, Color.green); //cameraForward
    }
}
