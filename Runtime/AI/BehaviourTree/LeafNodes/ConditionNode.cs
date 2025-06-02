using System;

namespace SpookyCore.Runtime.AI.BehaviourTree
{
    public class ConditionNode : NodeBase
    {
        private readonly Func<AIContext, bool> _condition;

        public ConditionNode(string name, Func<AIContext, bool> condition)
        {
            _condition = condition;
            Name = name;
        }
        
        public override NodeState Execute(AIContext context)
        {
            CurrentState = _condition(context) ? NodeState.Success : NodeState.Failure;
            return CurrentState;
        }

        public override NodeBase Clone()
        {
            return new ConditionNode(Name, _condition);
        }
    }
}