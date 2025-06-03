using System;

namespace SpookyCore.Runtime.AI
{
    public class ExecutionNode : NodeBase
    {
        private readonly Func<AIContext, NodeState> _action;
        
        public ExecutionNode(string name, Func<AIContext, NodeState> action)
        {
            _action = action;
            Name = name;
        }
        
        public override NodeState Execute(AIContext context)
        {
            CurrentState = _action(context);
            return CurrentState;
        }

        public override NodeBase Clone()
        {
            return new ExecutionNode(Name, _action);
        }
    }
}