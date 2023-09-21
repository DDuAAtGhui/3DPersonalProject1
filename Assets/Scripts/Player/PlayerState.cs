using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    string AnimationName;
    protected Rigidbody rb;
    protected float X_Input, Y_Input;

    protected float StateTimer = 0f;
    protected bool isAnimationFinishTriggerCalled;

    public PlayerState(Player player, PlayerStateMachine stateMachine, string AnimationName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.AnimationName = AnimationName;
        rb = player.rb;
    }
    public virtual void Enter()
    {
        Debug.Log("Enter State : " + AnimationName);
        player.anim.SetBool(AnimationName, true);
        isAnimationFinishTriggerCalled = false;
    }
    public virtual void Update()
    {
        Debug.Log("Update State : " + AnimationName);

        StateTimer -= Time.deltaTime;

        X_Input = Input.GetAxis("Horizontal");
        Y_Input = Input.GetAxis("Vertical");


    }
    public virtual void FixedUpdate()
    {
        Debug.Log("FixedUpdate State : " + AnimationName);

    }
    public virtual void Exit()
    {
        Debug.Log("Exit State : " + AnimationName);
        player.anim.SetBool(AnimationName, false);
    }

    public void AnimationFinishTrigger() => isAnimationFinishTriggerCalled = true;
}
