using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UnityEngine;
using UnityEngine.AI;

public class NormalZombieIdleBehaviour : StateMachineBehaviour
{
    float idleMotionChangeTimer = 0f;
    float transitionToMoveTimer = 0f;
    GameManager gameManager;
    NavMeshAgent navMeshAgent;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gameManager = GameManager.instance;
        navMeshAgent = animator.GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(navMeshAgent.transform.position);
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        idleMotionChangeTimer += Time.deltaTime;
        transitionToMoveTimer += Time.deltaTime;

        float R = Random.Range(-5f, 5f);
        if (idleMotionChangeTimer >= 10f)
        {
            idleMotionChangeTimer = 0f;
            animator.SetFloat("idleMotionPar", Mathf.Clamp(Random.Range(0f, 1f), 0.15f, 1f));
        }

        if (transitionToMoveTimer >= Random.Range(3f, 15f))
        {
            transitionToMoveTimer = 0f;
            animator.SetBool(gameManager.animIDisMove, true);
            navMeshAgent.SetDestination(animator.transform.position +
                new Vector3(R, R, R));
        }

        if (animator.GetComponentInChildren<FieldOfView>().isTargetFound(gameManager.player.gameObject))
            animator.SetBool(gameManager.animIDisMove, true);
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}
