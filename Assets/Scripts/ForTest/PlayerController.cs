using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController CC;

    [Header("Move Info")]
    [SerializeField] float MoveSpeed = 10f;

    float X_Input, Y_Input;
    private void Start()
    {
        CC = GetComponent<CharacterController>();
    }

    private void Update()
    {
        X_Input = Input.GetAxis("Horizontal");
        Y_Input = Input.GetAxis("Vertical");

        var MovementDirection = new Vector3(X_Input, 0, Y_Input).normalized;

        CC.Move(MovementDirection * MoveSpeed * Time.deltaTime);
    }
}
