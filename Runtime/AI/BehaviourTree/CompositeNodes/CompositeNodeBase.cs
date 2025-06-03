using System.Collections.Generic;

namespace SpookyCore.Runtime.AI
{
    public abstract class CompositeNodeBase : NodeBase
    {
        protected List<NodeBase> _children = new();
        protected int _currentChildIndex;

        public override NodeState Execute(AIContext context)
        {
            return NodeState.Running;
        }

        public override void AddChild(NodeBase child)
        {
            _children.Add(child);
        }

        public override List<NodeBase> GetChildren()
        {
            return _children;
        }

        public override void Reset()
        {
            base.Reset();
            _currentChildIndex = 0;
        }
    }
}