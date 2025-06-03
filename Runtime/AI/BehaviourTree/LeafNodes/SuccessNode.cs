namespace SpookyCore.Runtime.AI
{
    public class SuccessNode : NodeBase
    {
        public SuccessNode(string name)
        {
            Name = name;
        }
        
        public override NodeState Execute(AIContext context)
        {
            return NodeState.Success;
        }

        public override NodeBase Clone()
        {
            return new SuccessNode(Name);
        }
    }
}