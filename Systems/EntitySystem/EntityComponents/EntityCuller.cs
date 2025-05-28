using SpookyCore.Utilities.SmallSystems;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityCuller : EntityComponent
    {
        public bool ShouldBeCulled;
        public bool HasJustChangedCullState;
        private bool _lastCullState;
        protected Bounds _bounds;
        protected FrustumCullerSystem _cullerSystem;
        
        public override void OnStart()
        {
            //_bounds = Entity.Get<EntityCollider>().Collider.bounds;
            _cullerSystem = FrustumCullerSystem.Instance;
        }

        public override void OnUpdate()
        {
            ShouldBeCulled = _cullerSystem.ShouldBeCulled(transform.position, _bounds);
            HasJustChangedCullState = ShouldBeCulled != _lastCullState;
            _lastCullState = ShouldBeCulled;
        }
    }
}