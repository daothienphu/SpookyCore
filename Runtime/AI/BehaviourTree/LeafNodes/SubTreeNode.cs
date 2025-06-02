using System.Collections.Generic;

namespace SpookyCore.Runtime.AI.BehaviourTree
{
    public class SubTreeNode : NodeBase
    {
        private readonly NodeBase _subTree;

        public SubTreeNode(string name, NodeBase subTree)
        {
            _subTree = subTree;
            Name = name;
        }
        
        public override NodeState Execute(AIContext context)
        {
            CurrentState = _subTree.Execute(context);
            return CurrentState;
        }

        public override List<NodeBase> GetChildren()
        {
            return new List<NodeBase>{_subTree};
        }

        public override NodeBase Clone()
        {
            return new SubTreeNode(Name, _subTree.Clone());
        }
    }
}