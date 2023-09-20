using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    string AnimationName;
    protected Rigidbody rb;

    public float StateTimer = 0f;
    protected bool isAnimationFinishTriggerCalled;

    public PlayerState(Player player, PlayerStateMachine stateMachine, string AnimationName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.AnimationName = AnimationName;
    }
    public virtual void Enter()
    {
        Debug.Log("Enter State : " + AnimationName);
        player.anim.SetBool(AnimationName, true);
        isAnimationFinishTriggerCalled = false;
    }
    public virtual void Upadate()
    {
        Debug.Log("Update State : " + AnimationName);

        StateTimer -= Time.deltaTime;
    }
    public virtual void FixedUpadate()
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
