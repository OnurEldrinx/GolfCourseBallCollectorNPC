using BehaviourTree;

namespace BallCollectorNPC.AI
{
    public class CollectClosestBall : IStrategy
    {

        private readonly BallCollector _ballCollector;
        
        public CollectClosestBall(BallCollector ballCollector)
        {
            _ballCollector = ballCollector;
        }

        public Node.Status Process()
        {

            if (_ballCollector.targetBall is null && _ballCollector.collectedBall is null)
            {
                _ballCollector.targetBall = _ballCollector.ClosestBall();
            }

            if (_ballCollector.targetBall is not null)
            {
                _ballCollector.SetAgentDestination(_ballCollector.targetBall.transform.position);

                if (_ballCollector.DistanceTo(_ballCollector.targetBall.transform) < 0.5f)
                {
                    if (_ballCollector.collectedBall is null)
                    {
                        _ballCollector.CollectAnimation();
                        _ballCollector.CollectAnimationPlaying = true;
                        _ballCollector.collectedBall = _ballCollector.targetBall;
                        _ballCollector.BallCollected(_ballCollector.targetBall);
                        _ballCollector.targetBall = null;
                        _ballCollector.ResetAgentPath();
                        return Node.Status.Running;
                    }

                    if (_ballCollector.CollectAnimationPlaying)
                    {
                        return Node.Status.Running;
                    }

                    return Node.Status.Success;
                }
            }

            return Node.Status.Running;
        }
    }
}