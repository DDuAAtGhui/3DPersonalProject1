using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region components
    CharacterController CC;
    PlayerInputSystem playerInput;
    Animator anim;
    [SerializeField] CinemachineVirtualCamera Vcam;
    #endregion
    #region infos
    [Header("Move Info")]
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
    [SerializeField] Vector2 look;
    Vector3 vcamForward;
    [SerializeField] float Mouse_sensitivity = 1f;

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

        //키가 떼졌는지를 감지하는 콜백
        playerInput.CharacterControls.Move.canceled += onMoveAction;

        //조이스틱은 스틱으로 컨트롤하니까 perform필수임
        //근데 키보드도 결국 4방향만 할거 아니면 perform하는게 좋음
        //performed은 0과 1사이의 값을 콜백받음
        playerInput.CharacterControls.Move.performed += onMoveAction;


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
        if (isWalkPressed)
            CC.Move(currentWalkMovement * SpeedMultiplierToWalk * MoveSpeed * Time.deltaTime);

        else
            CC.Move(currentMovement * MoveSpeed * Time.deltaTime);

        handleAnimation();
        handleRotation();
    }
    private void FixedUpdate()
    {

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
        look = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Mouse_X = context.ReadValue<Vector2>().x * Mouse_sensitivity;
        Mouse_Y = context.ReadValue<Vector2>().y * Mouse_sensitivity;
    }
    void handleAnimation()
    {
        CurrentSpeed = CC.velocity.magnitude;

        anim.SetFloat("Speed", CurrentSpeed);
    }


    void handleRotation()
    {
        vcamForward = Vcam.transform.forward;


        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentMovement);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }
}
