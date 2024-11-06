namespace BehaviourTree
{
    public class BehaviourTree : Node {
        private readonly IPolicy _policy;
        
        public BehaviourTree(string name, IPolicy policy = null) : base(name) {
            _policy = policy ?? Policies.RunForever;
        }

        public override Status Process() {
            Status status = Children[CurrentChild].Process();
            if (_policy.ShouldReturn(status)) {
                return status;
            }
            
            CurrentChild = (CurrentChild + 1) % Children.Count;
            return Status.Running;
        }

        /*public void PrintTree() {
            StringBuilder sb = new StringBuilder();
            PrintNode(this, 0, sb);
            Debug.Log(sb.ToString());
        }

        static void PrintNode(Node node, int indentLevel, StringBuilder sb) {
            sb.Append(' ', indentLevel * 2).AppendLine(node.name);
            foreach (Node child in node.children) {
                PrintNode(child, indentLevel + 1, sb);
            }
        }*/
    }
}