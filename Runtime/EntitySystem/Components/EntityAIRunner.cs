using System.Collections.Generic;
using SpookyCore.Runtime.AI.BehaviourTree;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
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
        }

        #endregion
    }
}