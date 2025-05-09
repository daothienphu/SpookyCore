using UnityEngine;

namespace SpookyCore.Utilities.SmallSystems
{
    public class FrustumCullerSystem : MonoSingleton<FrustumCullerSystem>
    {
        private Camera _camera;
        private Plane[] _frustumPlanes;
        private float _cullDistance;
        private Vector3 _cameraPosition;

        protected override void OnAwake()
        {
            base.OnAwake();
            _camera = Camera.main;
            _cullDistance = _camera.orthographicSize * Screen.width / Screen.height * 1.1f;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!_camera)
            {
                _camera = Camera.main;
            }

            _cameraPosition = new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0f);
            _frustumPlanes = GeometryUtility.CalculateFrustumPlanes(_camera);
        }

        public bool ShouldBeCulled(Vector3 position, Bounds bounds)
        {
            if (!_camera)
            {
                _camera = Camera.main;
            }
            var distanceToCamera = Vector3.Distance(_cameraPosition, position);
            if (distanceToCamera > _cullDistance)
            {
                return true;
            }
            
            //return !GeometryUtility.TestPlanesAABB(_frustumPlanes, bounds);
            return false;
        }
    }
}