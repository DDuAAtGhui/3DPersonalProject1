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
    [SerializeField] float SpeedMultiplierToRun = 2.5f;
    #endregion

    //current : 월드 좌표계 중심으로 움직임
    Vector2 currentMovementInput;
    Vector3 currentMovement;

    //local : current 기준으로 내 캐릭터 좌표계 중심으로 움직이게 할거임
    Vector3 localMovement;
    bool isMovementPressed;

    Vector3 currentRunMovement;
    bool isRunPressed;

    [Header("Rotate Info")]
    //1프레임당 회전 감소율
    [SerializeField] float rotationFactoerPerFram = 2.5f;

    [Header("Camera Info")]
    [SerializeField] float Mouse_X, Mouse_Y;
    [SerializeField] float Mouse_sensitivity = 1f;
    private void Awake()
    {
        CC = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();


        playerInput = new PlayerInputSystem();

        //인스턴스 생성
        //started 콜백에 onMovementInput 메소드 등록
        playerInput.CharacterControls.Move.started += onMovementInput;

        //키가 떼졌는지를 감지하는 콜백
        playerInput.CharacterControls.Move.canceled += onMovementInput;

        //조이스틱은 스틱으로 컨트롤하니까 perform필수임
        //근데 키보드도 결국 4방향만 할거 아니면 perform하는게 좋음
        //performed은 0과 1사이의 값을 콜백받음
        playerInput.CharacterControls.Move.performed += onMovementInput;


        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;

        playerInput.CharacterControls.Look.performed += onLook;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    //Move 액션 입력을 감지하고 started콜백 받아서 context => 이하의 함수를 수행한다

    private void Update()
    {
        if (isRunPressed)
            CC.Move(localMovement * SpeedMultiplierToRun * MoveSpeed * Time.deltaTime);

        else
            CC.Move(currentMovement * MoveSpeed * Time.deltaTime);

        handleRotation();
        handleAnimation();
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

    void onMovementInput(InputAction.CallbackContext context)
    {
        //WASD 입력값 저장
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;

        currentRunMovement.x = currentMovementInput.x * SpeedMultiplierToRun;
        currentRunMovement.z = currentMovementInput.y * SpeedMultiplierToRun;
        //WASD중에 하나라도 눌렸는지 체크
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;

        //로컬 기준 움직임으로 전환
        localMovement = transform.TransformDirection(new Vector3(currentMovementInput.x, 0, currentMovementInput.y));
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void onLook(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>() * Mouse_sensitivity);
    }
    void handleAnimation()
    {
        float CurrentSpeed;

        if (isRunPressed)
            CurrentSpeed = Mathf.Abs(localMovement.x) * MoveSpeed * SpeedMultiplierToRun + Mathf.Abs(localMovement.z) * MoveSpeed * SpeedMultiplierToRun;

        else
            CurrentSpeed = Mathf.Abs(localMovement.x) * MoveSpeed + Mathf.Abs(localMovement.z) * MoveSpeed;

        anim.SetFloat("Speed", CurrentSpeed);
    }


    void handleRotation()
    {
        Vector3 position2LookAt = new Vector3(currentMovementInput.x, 0, currentMovementInput.y);

        Quaternion currentRotation = transform.rotation;

        //if (isMovementPressed)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(position2LookAt);
        //    transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactoerPerFram * Time.deltaTime);

        //}
        //
    }

}
