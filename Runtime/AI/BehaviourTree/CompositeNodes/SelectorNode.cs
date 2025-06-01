
namespace SpookyCore.BehaviourTree
{
    public class SelectorNode : CompositeNodeBase
    {
        public SelectorNode(string name)
        {
            Name = name;
        }
        
        public override NodeState Execute(AIContext context)
        {
            foreach (var child in _children)
            {
                var executionResult = child.Execute(context);
                if (executionResult is NodeState.Success or NodeState.Running)
                {
                    CurrentState = executionResult;
                    return CurrentState;
                }
            }

            CurrentState = NodeState.Failure;
            return CurrentState;
        }

        public override NodeBase Clone()
        {
            var selectorNode = new SelectorNode(Name);
            foreach (var child in _children)
            {
                selectorNode.AddChild(child.Clone());
            }

            return selectorNode;
        }
    }
}