using System;
using System.Collections.Generic;

namespace SpookyCore.BehaviourTree
{
    public class FluentTreeBuilder
    {
        private Stack<NodeBase> _nodeStack = new();
        private NodeBase _root;

        public FluentTreeBuilder Sequence(string name = "")
        {
            var sequenceNode = new SequenceNode(name);
            AddNode(sequenceNode);
            _nodeStack.Push(sequenceNode);
            return this;
        }

        public FluentTreeBuilder Selector(string name = "")
        {
            var selectorNode = new SelectorNode(name);
            AddNode(selectorNode);
            _nodeStack.Push(selectorNode);
            return this;
        }

        public FluentTreeBuilder ReactiveSequence(string name)
        {
            var sequenceNode = new ReactiveSequenceNode(name);
            AddNode(sequenceNode);
            _nodeStack.Push(sequenceNode);
            return this;
        }

        public FluentTreeBuilder ReactiveSelector(string name = "")
        {
            var selectorNode = new ReactiveSelectorNode(name);
            AddNode(selectorNode);
            _nodeStack.Push(selectorNode);
            return this;
        }

        public FluentTreeBuilder Condition(string name, Func<AIContext, bool> condition)
        {
            AddNode(new ConditionNode(name, condition));
            return this;
        }

        public FluentTreeBuilder Execution(string name, Func<AIContext, NodeState> action)
        {
            AddNode(new ExecutionNode(name, action));
            return this;
        }

        public FluentTreeBuilder Failure(string name)
        {
            AddNode(new FailureNode(name));
            return this;
        }

        public FluentTreeBuilder Success(string name)
        {
            AddNode(new SuccessNode(name));
            return this;
        }

        public FluentTreeBuilder SubTree(string name, NodeBase subTree)
        {
            AddNode(new SubTreeNode(name, subTree));
            return this;
        }

        public FluentTreeBuilder End()
        {
            if (_nodeStack.Count > 0)
            {
                _nodeStack.Pop();
            }

            return this;
        }

        public NodeBase Build()
        {
            return _root;
        }
        
        private void AddNode(NodeBase node)
        {
            if (_nodeStack.Count > 0)
            {
                _nodeStack.Peek().AddChild(node);
            }
            else
            {
                _root = node;
            }
        }
    }
}