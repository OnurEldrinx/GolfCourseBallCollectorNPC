namespace BehaviourTree
{
    public class Leaf : Node {
        private readonly IStrategy _strategy;

        public Leaf(string name, IStrategy strategy, int priority = 0) : base(name, priority) {
            _strategy = strategy;
        }
        
        public override Status Process() => _strategy.Process();

        protected override void Reset() => _strategy.Reset();
    }
}