using UnityEngine;

namespace SpookyCore.Runtime.AI.BehaviourTree
{
    public abstract class AIBehavior : ScriptableObject
    {
        public NodeState CurrentState;
        public NodeBase RootNode;

        public abstract void OnStart(AIContext context);

        public virtual NodeState Execute(AIContext context)
        {
            CurrentState = RootNode?.Execute(context) ?? NodeState.Failure;
            return CurrentState;
        }
    }
}