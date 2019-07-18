using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill_after_animation : StateMachineBehaviour {

    // target hit animation object is destroyed after the animation is complete
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Destroy(animator.gameObject);
    }
}
