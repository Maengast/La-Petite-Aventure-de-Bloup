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
        boss = animator.gameObject.GetComponent<Boss>();
        player = GameObject.FindObjectOfType<Player>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // int currentPoint = boss.GetCurrentPoint();
        // if (boss.GetPath() != null &&  currentPoint < boss.GetPath().Count)
        // {
        //     
        //
        //     //boss.Move(targetPosition);
        //     Node targetNode = boss.GetPath()[currentPoint];
        //     Vector3 targetPosition = targetNode.Position;
        //     if (targetNode.LinkType == PathLinkType.jump || targetNode.LinkType == PathLinkType.runoff)
        //     {
        //         boss.Jump();
        //     }


            //if (Vector2.Distance(boss.transform.position, player.transform.position) <= 1f)
            //{
            //    IAttack test = AttackFactory.GetAttack("Claws");
            //    boss.Attack(test);
            //    animator.SetTrigger("Attack");
            //}
        //}

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
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
