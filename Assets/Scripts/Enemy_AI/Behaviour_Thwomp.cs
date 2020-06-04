using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// worthless so far.
public class Behaviour_Thwomp : StateMachineBehaviour
{
    AnimatorStateInfo currentStateInfo;
    AnimatorStateInfo previousStateInfo;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        previousStateInfo = animator.GetCurrentAnimatorStateInfo(0);
    }
}
