namespace BehaviourTree
{
    public interface IStrategy {
        Node.Status Process();

        void Reset() {
            // No ops
        }
    }
}