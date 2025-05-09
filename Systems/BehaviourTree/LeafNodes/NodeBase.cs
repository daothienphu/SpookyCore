using System.Collections.Generic;

namespace SpookyCore.BehaviourTree
{
    public enum NodeState
    {
        None,
        Running,
        Success,
        Failure
    }
    
    public abstract class NodeBase
    {
        public NodeState CurrentState = NodeState.None;
        public string Name;
        
        public abstract NodeState Execute(AIContext context);

        public virtual void AddChild(NodeBase child) { }

        public virtual List<NodeBase> GetChildren()
        {
            return new List<NodeBase>();
        }

        public virtual void Reset()
        {
            CurrentState = NodeState.None;
        }

        public abstract NodeBase Clone();
    }
}