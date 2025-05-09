namespace SpookyCore.BehaviourTree
{
    public class FailureNode : NodeBase
    {
        public FailureNode(string name)
        {
            Name = name;
        }
        
        public override NodeState Execute(AIContext context)
        {
            return NodeState.Failure;
        }

        public override NodeBase Clone()
        {
            return new FailureNode(Name);
        }
    }
}