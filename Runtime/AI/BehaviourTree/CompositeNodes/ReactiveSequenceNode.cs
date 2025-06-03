namespace SpookyCore.Runtime.AI
{
    public class ReactiveSequenceNode : CompositeNodeBase
    {
        public ReactiveSequenceNode(string name)
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

                if (state == NodeState.Failure)
                {
                    CurrentState = NodeState.Failure;
                    return CurrentState;
                }

                _currentChildIndex++;
            }

            Reset();
            return NodeState.Success;
        }

        public override NodeBase Clone()
        {
            var sequenceNode = new ReactiveSequenceNode(Name);
            foreach (var child in _children)
            {
                sequenceNode.AddChild(child.Clone());
            }
            return sequenceNode;
        }
    }
}