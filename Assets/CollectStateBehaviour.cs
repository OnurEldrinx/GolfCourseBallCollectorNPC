using UnityEngine;

public class CollectStateBehaviour : StateMachineBehaviour
{
    public bool collecting;
    public bool gettingUp;
    private BallCollector _ballCollector;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _ballCollector = animator.transform.GetComponent<BallCollector>();
        
        if (collecting)
        {
            _ballCollector.CollectAnimationPlaying = true;
        }
        
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        if (gettingUp)
        {
            _ballCollector.CollectAnimationPlaying = false;
        }else if (collecting)
        {
            _ballCollector.AttachBallToHand();
        }
    }
}
