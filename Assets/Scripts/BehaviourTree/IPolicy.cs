namespace BehaviourTree
{
    public interface IPolicy {
        bool ShouldReturn(Node.Status status);
    }
}