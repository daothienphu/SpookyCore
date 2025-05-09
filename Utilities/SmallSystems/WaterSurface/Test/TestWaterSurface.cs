using UnityEngine;

namespace SpookyCore.Utilities
{
    public class TestWaterSurface : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private WaterSurface _waterSurface;
        [Range(0.01f, 5f)] [SerializeField] private float _speed;
        [SerializeField] private Transform _splasherObject;

        private float _lastPosY;
        private float _currentPosY;

        private void Start()
        {
            AdjustCameraToFitTheWater();
        }

        private void Update()
        {
            var worldPoint = GameCache.Camera.ScreenToWorldPoint(Input.mousePosition);
            worldPoint.z = -1f;
            _splasherObject.position = worldPoint;
            
            var surfaceLevel = _waterSurface.GetSurfaceLevel();
            var splasherPosition = _splasherObject.position;
            _currentPosY = splasherPosition.y;

            if ((_lastPosY - surfaceLevel) * (_currentPosY - surfaceLevel) < 0)
            {
                var speed = (_currentPosY - _lastPosY) / 2 * 5;
                _waterSurface.CreateSplashAt(splasherPosition.x, speed);
            }

            _lastPosY = _currentPosY;
        }
        
        private void AdjustCameraToFitTheWater()
        {
            var waterWidth = _waterSurface.WaterSurfaceSettings.ColumnCount * _waterSurface.WaterSurfaceSettings.ColumnWidth;
            var waterMin = _waterSurface.transform.position.x;
            var waterMax = _waterSurface.transform.position.x + waterWidth;
            

            // Adjust the camera's orthographic size and position
            var cameraHeight = _camera.orthographicSize * 2;
            var aspectRatio = _camera.aspect;
            var cameraWidth = cameraHeight * aspectRatio;

            if (waterWidth > cameraWidth)
            {
                _camera.orthographicSize = waterWidth / (2 * aspectRatio);
            }

            var centerX = (waterMax + waterMin) / 2;
            _camera.transform.position = new Vector3(
                centerX, 
                _camera.transform.position.y + _waterSurface.WaterSurfaceSettings.ColumnHeight, 
                _camera.transform.position.z);
        }
    }
}