using System.Collections.Generic;
using System.Linq;

namespace BehaviourTree
{
    public class PrioritySelector : Selector {
        private List<Node> _sortedChildren;
        List<Node> SortedChildren => _sortedChildren ??= SortChildren();
        
        protected virtual List<Node> SortChildren() => Children.OrderByDescending(child => child.Priority).ToList();
        
        public PrioritySelector(string name, int priority = 0) : base(name, priority) { }

        protected override void Reset() {
            base.Reset();
            _sortedChildren = null;
        }
        
        public override Status Process() {
            foreach (var child in SortedChildren) {
                switch (child.Process()) {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        Reset();
                        return Status.Success;
                    default:
                        continue;
                }
            }

            Reset();
            return Status.Failure;
        }
    }
}