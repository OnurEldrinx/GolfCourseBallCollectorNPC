using System;

namespace BehaviourTree {
    
    public class ActionStrategy : IStrategy {
        private readonly Action _doSomething;
        
        public ActionStrategy(Action doSomething) {
            this._doSomething = doSomething;
        }
        
        public Node.Status Process() {
            _doSomething();
            return Node.Status.Success;
        }
    }

    public class Condition : IStrategy {
        private readonly Func<bool> _predicate;
        
        public Condition(Func<bool> predicate) {
            this._predicate = predicate;
        }
        
        public Node.Status Process() => _predicate() ? Node.Status.Success : Node.Status.Failure;
    }
    
}
