using BehaviourTree;

namespace BallCollectorNPC.AI
{
    public class CheckHealth : IStrategy
    {

        private readonly BallCollector _ballCollector;
        
        public CheckHealth(BallCollector ballCollector)
        {
            _ballCollector = ballCollector;
        }
        
        public Node.Status Process()
        {
            return _ballCollector.EnoughHealth() ? Node.Status.Success : Node.Status.Failure;
        }
    }
}
