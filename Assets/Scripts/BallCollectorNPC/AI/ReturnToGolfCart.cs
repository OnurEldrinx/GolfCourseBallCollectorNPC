using BehaviourTree;

namespace BallCollectorNPC.AI
{
    public class ReturnToGolfCart : IStrategy
    {
        private readonly BallCollector _ballCollector;
        
        public ReturnToGolfCart(BallCollector ballCollector)
        {
            _ballCollector = ballCollector;
        }

        public Node.Status Process()
        {

            if (_ballCollector.collectedBall is null)
            {
                return Node.Status.Failure;
            }
            
            _ballCollector.ReturnToGolfCart();

            if (_ballCollector.DistanceToGolfCart() < 0.25f)
            {
                if (_ballCollector.collectedBall is not null)
                {
                    _ballCollector.DropBall();
                    //_ballCollector.collectedBall = null;
                    _ballCollector.ResetAgentPath();
                    return Node.Status.Success;
                }
            }

            return Node.Status.Running;

        }
    }
}