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

    //current : ���� ��ǥ�� �߽����� ������
    Vector2 currentMovementInput;
    Vector3 currentMovement;

    //local : current �������� �� ĳ���� ��ǥ�� �߽����� �����̰� �Ұ���
    Vector3 localMovement;
    bool isMovementPressed;

    Vector3 currentRunMovement;
    bool isRunPressed;

    [Header("Rotate Info")]
    //1�����Ӵ� ȸ�� ������
    [SerializeField] float rotationFactoerPerFram = 2.5f;

    [Header("Camera Info")]
    [SerializeField] float Mouse_X, Mouse_Y;
    [SerializeField] float Mouse_sensitivity = 1f;
    private void Awake()
    {
        CC = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();


        playerInput = new PlayerInputSystem();

        //�ν��Ͻ� ����
        //started �ݹ鿡 onMovementInput �޼ҵ� ���
        playerInput.CharacterControls.Move.started += onMovementInput;

        //Ű�� ���������� �����ϴ� �ݹ�
        playerInput.CharacterControls.Move.canceled += onMovementInput;

        //���̽�ƽ�� ��ƽ���� ��Ʈ���ϴϱ� perform�ʼ���
        //�ٵ� Ű���嵵 �ᱹ 4���⸸ �Ұ� �ƴϸ� perform�ϴ°� ����
        //performed�� 0�� 1������ ���� �ݹ����
        playerInput.CharacterControls.Move.performed += onMovementInput;


        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;

        playerInput.CharacterControls.Look.performed += onLook;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    //Move �׼� �Է��� �����ϰ� started�ݹ� �޾Ƽ� context => ������ �Լ��� �����Ѵ�

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
    //��ǲ�ý��� �� �� �׻� Ű�� ���� �������
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
        //WASD �Է°� ����
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;

        currentRunMovement.x = currentMovementInput.x * SpeedMultiplierToRun;
        currentRunMovement.z = currentMovementInput.y * SpeedMultiplierToRun;
        //WASD�߿� �ϳ��� ���ȴ��� üũ
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;

        //���� ���� ���������� ��ȯ
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
