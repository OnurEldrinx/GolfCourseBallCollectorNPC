using System.Collections.Generic;

namespace BehaviourTree {
    
    public class Node {
        public enum Status { Success, Failure, Running }
        
        public readonly string Name;
        public readonly int Priority;

        protected readonly List<Node> Children = new();
        protected int CurrentChild;

        protected Node(string name = "Node", int priority = 0) {
            Name = name;
            Priority = priority;
        }
        
        public void AddChild(Node child) => Children.Add(child);
        
        public virtual Status Process() => Children[CurrentChild].Process();

        protected virtual void Reset() {
            CurrentChild = 0;
            foreach (var child in Children) {
                child.Reset();
            }
        }
    }
    
}