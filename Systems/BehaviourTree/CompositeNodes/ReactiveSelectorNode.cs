namespace SpookyCore.BehaviourTree
{
    public class ReactiveSelectorNode : CompositeNodeBase
    {
        public ReactiveSelectorNode(string name)
        {
            Name = name;
        }

        public override NodeState Execute(AIContext context)
        {
            while (_currentChildIndex < _children.Count)
            {
                var state = _children[_currentChildIndex].Execute(context);

                if (state == NodeState.Running)
                {
                    CurrentState = NodeState.Running;
                    return CurrentState;
                }

                if (state == NodeState.Success)
                {
                    CurrentState = NodeState.Success;
                    return CurrentState;
                }

                _currentChildIndex++;
            }
            
            Reset();
            return NodeState.Failure;
        }


        public override NodeBase Clone()
        {
            var selectorNode = new ReactiveSelectorNode(Name);
            foreach (var child in _children)
            {
                selectorNode.AddChild(child.Clone());
            }

            return selectorNode;
        }
    }
}