using UnityEngine;
using System.Collections;

public class ActorExtendedIdleController : StateMachineBehaviour {

    public string parameterName = "ext_idle_index";
    public int animationsNumber = 0;

    private int lastRandoIndex = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int currentRandomIndex;

        do
        {
            currentRandomIndex = Random.Range(1, animationsNumber + 1);
        } while (lastRandoIndex == currentRandomIndex);

        animator.SetInteger(parameterName, currentRandomIndex);
        lastRandoIndex = currentRandomIndex;                      
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
