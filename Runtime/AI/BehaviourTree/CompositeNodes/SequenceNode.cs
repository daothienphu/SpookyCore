namespace SpookyCore.Runtime.AI
{
    public class SequenceNode : CompositeNodeBase
    {
        public SequenceNode(string name)
        {
            Name = name;
        }
        
        public override NodeState Execute(AIContext context)
        {
            foreach (var child in _children)
            {
                var executionResult = child.Execute(context);
                if (executionResult is NodeState.Failure or NodeState.Running)
                {
                    CurrentState = executionResult;
                    return CurrentState;
                }
            }

            CurrentState = NodeState.Success;
            return CurrentState;
        }

        public override NodeBase Clone()
        {
            var sequenceNode = new SequenceNode(Name);
            foreach (var child in _children)
            {
                sequenceNode.AddChild(child.Clone());
            }
            return sequenceNode;
        }
    }
}