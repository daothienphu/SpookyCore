namespace SpookyCore.BehaviourTree
{
    public class RootNode : NodeBase
    {
        private NodeBase _child;
        
        public override NodeState Execute(AIContext context)
        {
            return _child.Execute(context);
        }

        public override void AddChild(NodeBase child)
        {
            _child = child;
        }

        public override NodeBase Clone()
        {
            var rootNode = new RootNode();
            rootNode.AddChild(_child.Clone());
            return rootNode;
        }
    }
}