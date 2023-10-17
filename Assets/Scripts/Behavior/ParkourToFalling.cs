using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourToFalling : StateMachineBehaviour
{
    [Tooltip("애니메이션 몇초부터 낙하시간 측정할건지")][SerializeField] float whenFallingTimerStart = 0.0f;
    [Tooltip("이 시간만큼 fallingTime시간 결과값 감소")][SerializeField] float TimerDamping;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager.instance.player.totalFallingTime -= whenFallingTimerStart;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager.instance.player.totalFallingTime += Time.deltaTime;

        //if (fallingTimer - TimerDamping >= 0f && GameManager.instance.Log_PlayerTotalFallingTime)
        //    Debug.Log("파쿠르 낙하 시간 : " + fallingTimer);   
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
