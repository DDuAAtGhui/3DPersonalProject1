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
    //1�����Ӵ� ȸ�� ������
    [SerializeField] float rotationFactorPerFrame = 25f;

    [Header("Input Info")]

    [Header("Camera Info")]
    [SerializeField][Tooltip("���콺 X�� ����ӵ�")] float Mouse_X;
    [SerializeField][Tooltip("���콺 Y�� ����ӵ�")] float Mouse_Y;
    [SerializeField] float Mouse_sensitivity = 1f;
    [SerializeField] Vector3 look;
    [SerializeField] Vector3 cameraForward;

    #endregion

    //current : ���� ��ǥ�� �߽����� ������
    Vector2 currentMovementInput;
    Vector3 currentMovement;

    //local : current �������� �� ĳ���� ��ǥ�� �߽����� �����̰� �Ұ���
    Vector3 localMovement;
    bool isMovementPressed;

    Vector3 currentWalkMovement;
    bool isWalkPressed;

    private void Awake()
    {
        CC = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();


        playerInput = new PlayerInputSystem();

        //�ν��Ͻ� ����
        //started �ݹ鿡 onMoveAction �޼ҵ� ���
        playerInput.CharacterControls.Move.started += onMoveAction;
        //���̽�ƽ�� ��ƽ���� ��Ʈ���ϴϱ� perform�ʼ���
        //�ٵ� Ű���嵵 �ᱹ 4���⸸ �Ұ� �ƴϸ� perform�ϴ°� ����
        //performed�� 0�� 1������ ���� �ݹ����
        playerInput.CharacterControls.Move.performed += onMoveAction;
        //Ű�� ���������� �����ϴ� �ݹ�
        playerInput.CharacterControls.Move.canceled += onMoveAction;


        playerInput.CharacterControls.Walk.started += onWalkAction;
        playerInput.CharacterControls.Walk.canceled += onWalkAction;

        playerInput.CharacterControls.Look.performed += onLookAction;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    //Move �׼� �Է��� �����ϰ� started�ݹ� �޾Ƽ� context => ������ �Լ��� �����Ѵ�

    private void Update()
    {
        handleCamera();

        // WASD �Է� ����
        if (isMovementPressed)
        {
            //cameraForward �������� �Է� ���� y�� �� + �ڽ� ������Ʈ�� �÷��̾ �׻� �ٶ󺸴µ� �ű��� ������ ���Ϳ� x���Է°� ����
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

    //��ǲ�ý��� �� �� �׻� Ű�� ���� �������
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
        //WASD �Է°� ����
        currentMovementInput = context.ReadValue<Vector2>();

        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;

        //currentMovemnt�� cameraForward �������� ����(ī�޶� �÷��̾ �ٶ󺸴� ���� = �÷��̾ ���ӿ��� ���� ����)
        currentMovement = Quaternion.LookRotation(cameraForward) * currentMovement;


        currentWalkMovement.x = currentMovementInput.x * SpeedMultiplierToWalk;
        currentWalkMovement.z = currentMovementInput.y * SpeedMultiplierToWalk;

        //WASD�߿� �ϳ��� ���ȴ��� üũ
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;

        //���� ���� ���������� ��ȯ
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
        //B - A : A�� B�� �ٶ󺸴� ������ ���Ͱ�
        //A - B : B�� A�� �ٶ󺸴� ������ ���Ͱ�
        look = (transform.position - Vcam_FreeLook.transform.position).normalized;

        //y�� �÷��̾�� �����ϰ��ؼ� 2���� ���ó�� ������ �� �ְ� ���� ���Ͱ� ����
        cameraForward = (transform.position - new Vector3(Vcam_FreeLook.transform.position.x, transform.position.y, Vcam_FreeLook.transform.position.z)).normalized;

        Debug.DrawLine(Vcam_FreeLook.transform.position, transform.position, Color.red); // look
        Debug.DrawLine(new Vector3(Vcam_FreeLook.transform.position.x, transform.position.y, Vcam_FreeLook.transform.position.z), transform.position, Color.green); //cameraForward
    }
}
