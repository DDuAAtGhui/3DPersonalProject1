using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected Rigidbody rb;
    protected float X_Input, Y_Input;

    protected float StateTimer = 0f;
    protected bool isAnimationFinishTriggerCalled;

    public PlayerState(Player player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        rb = player.rb;
    }
    public virtual void Enter()
    {
        isAnimationFinishTriggerCalled = false;
    }
    public virtual void Update()
    {

        StateTimer -= Time.deltaTime;

        X_Input = Input.GetAxis("Horizontal");
        Y_Input = Input.GetAxis("Vertical");


    }
    public virtual void FixedUpdate()
    {

    }
    public virtual void Exit()
    {
    }

    public void AnimationFinishTrigger() => isAnimationFinishTriggerCalled = true;
}
