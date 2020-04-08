using PathFinder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_run : StateMachineBehaviour
{

    Boss boss;
    Player player;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = GameObject.FindObjectOfType<Boss>();
        player = GameObject.FindObjectOfType<Player>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(boss.GetPath() != null && boss.OnGround)
        {
            Vector3 targetPosition = boss.GetNodeToFollow().Position;
            if (boss.GetNodeToFollow().LinkType == PathLinkType.jump || boss.GetNodeToFollow().LinkType == PathLinkType.runoff)
            {
                boss.Jump();
            }

            else if(boss.GetNodeToFollow().LinkType == PathLinkType.fall)
            {
                targetPosition.y = boss.transform.position.y;

            }

            boss.Move(targetPosition);

            //if (Vector2.Distance(boss.transform.position, player.transform.position) <= 10f)
            //{
            //    IAttack test = ScriptableObject.CreateInstance<CircularSawAttack>();
            //    boss.Attack(test);
            //    animator.SetTrigger("Attack");
            //}
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.ResetTrigger("Attack");
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
