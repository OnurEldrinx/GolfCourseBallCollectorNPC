namespace BehaviourTree
{
    public class Sequence : Node {
        public Sequence(string name, int priority = 0) : base(name, priority) { }

        public override Status Process() {
            if (CurrentChild < Children.Count) {
                switch (Children[CurrentChild].Process()) {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        CurrentChild = 0;
                        return Status.Failure;
                    default:
                        CurrentChild++;
                        return CurrentChild == Children.Count ? Status.Success : Status.Running;
                }
            }

            Reset();
            return Status.Success;
        }
    }
}