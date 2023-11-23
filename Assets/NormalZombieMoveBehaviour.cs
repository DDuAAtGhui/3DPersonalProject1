using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NormalZombieMoveBehaviour : StateMachineBehaviour
{
    [SerializeField] float roamingTimer = 0f;
    float startViewAngle;
    float startViewRadius;
    GameManager gameManager;
    Player player;
    FieldOfView fov;
    NavMeshAgent navMeshAgent;
    Rigidbody rb;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gameManager = GameManager.instance;
        player = gameManager.player;
        fov = animator.GetComponentInChildren<FieldOfView>();
        navMeshAgent = animator.GetComponent<NavMeshAgent>();
        rb = animator.GetComponent<Rigidbody>();
        startViewAngle = fov.viewAngle;
        startViewRadius = fov.viewRadius;
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        roamingTimer += Time.deltaTime;

        if (roamingTimer >= Random.Range(7f, 15f) &&
            !fov.isTargetFound(player.gameObject) && animator.GetComponent<Enemy>().isWandering)
        {
            roamingTimer = 0f;
            animator.SetBool(gameManager.animIDisMove, false);
        }

        if (fov.isTargetFound(player.gameObject))
        {
            roamingTimer = 0f;

            if (navMeshAgent.enabled)
                navMeshAgent.SetDestination(player.transform.position);

            //animator.transform.rotation =
            //    Quaternion.LookRotation(gameManager.player.transform.position);

            fov.viewRadius = startViewRadius + 3f;
            fov.viewAngle = 360f;

            if (navMeshAgent.remainingDistance <= 1f)
            {
                animator.SetBool(gameManager.animIDisAttack, true);
                animator.SetBool(gameManager.animIDisMove, false);
            }
        }
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fov.viewRadius = startViewRadius;
        fov.viewAngle = startViewAngle;
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
