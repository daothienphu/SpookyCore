using System.Collections.Generic;
using SpookyCore.BehaviourTree;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityAIRunner : EntityComponent
    {
        #region Fields

        [SerializeField] private float _behaviorTreeUpdateInterval = 0.5f;
        public List<AIBehavior> Behaviours;
        public NodeState CurrentState { get; set; }
        
        private float _behaviorTreeUpdateTimer;
        public NodeBase MainTree;

        #endregion

        #region Life Cycle

        public override void OnStart()
        {
            foreach (var behaviour in Behaviours)
            {
                behaviour.OnStart(Entity.AIContext);
            }
            
            SetupTree();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _behaviorTreeUpdateTimer += Time.deltaTime;
            if (_behaviorTreeUpdateTimer > _behaviorTreeUpdateInterval)
            {
                _behaviorTreeUpdateTimer = 0;
                CurrentState = MainTree.Execute(Entity.AIContext);
                //Debug.Log($"executed tree: {state}");
            }
        }

        #endregion

        #region Private Methods

        private void SetupTree()
        {
            var builder = new FluentTreeBuilder();
            var treeBuilder = builder.Selector();
            foreach (var behaviour in Behaviours)
            {
                treeBuilder = treeBuilder.SubTree($"{behaviour.name}", behaviour.RootNode.Clone());
            }

            MainTree = treeBuilder.End().Build();
            //_mainTree.LogTree();
        }

        #endregion
        
        //FROM OLD IMPLEMENTATION
        // [field: SerializeField] public BehaviorTree MainTree;
        // [SerializeField] private List<BehaviorTree> _subTrees = new();
        // [SerializeField] private int _ticksPerSecond;
        //
        // private float _timeBetweenTicks;
        // private float _timer;
        //
        // public override void OnStart()
        // {
        //     base.OnStart();
        //     _timeBetweenTicks = 1f / _ticksPerSecond;
        // }
        //
        // public override void OnUpdate()
        // {
        //     base.OnUpdate();
        //     if (MainTree == null)
        //     {
        //         Debug.Log("Main tree is null");
        //         return;
        //     }
        //     _timer -= Time.deltaTime;
        //     
        //     if (_timer <= 0)
        //     {
        //         MainTree.Evaluate();
        //         _timer = _timeBetweenTicks;
        //     }
        // }
        //
        // public void RebuildTree()
        // {
        //     RootNode root;
        //     if (MainTree.Root == null)
        //     {
        //         root = MainTree.CreateNode(typeof(RootNode), Vector2.zero) as RootNode;
        //         MainTree.Root = root;
        //     }
        //     else
        //     {
        //         root = MainTree.Root as RootNode;
        //     }
        //     
        //     SelectorNode selectorNode;
        //     if (root.Child == null)
        //     {
        //         selectorNode = MainTree.CreateNode(typeof(SelectorNode), Vector2.zero) as SelectorNode;
        //         MainTree.AddChild(MainTree.Root, selectorNode);
        //     }
        //     else
        //     {
        //         selectorNode = root.Child as SelectorNode;
        //     }
        //     
        //     if (selectorNode.Children is { Count: > 0 })
        //     {
        //         foreach (var child in selectorNode.Children)
        //         {
        //             if (child != null)
        //             {
        //                 AssetDatabase.RemoveObjectFromAsset(child);
        //             }
        //         }
        //     }
        //
        //     selectorNode.Children = new();
        //     
        //     foreach (var tree in _subTrees)
        //     {
        //         MainTree.SubTree(selectorNode, tree);
        //     }
        //     AssetDatabase.SaveAssets();
        // }
    }
}