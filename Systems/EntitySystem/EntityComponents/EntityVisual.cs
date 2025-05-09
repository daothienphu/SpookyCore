using System;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityVisual : EntityComponentBase
    {
        #region Enums

        [Serializable]
        public enum RotationModes
        {
            TimeBased,
            IncrementBased,
        }

        #endregion
        
        #region Action

        private Action OnRotationFinished = delegate { };

        #endregion

        #region Fields

        public Transform VisualTransform;
        public Vector2 Heading;
        public bool IsRotating { get; private set; }
        
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Transform _mainVisualTransform; //Used for mechanim, the _renderer is still used for some entities so it's still there, will be deleted soon
        [SerializeField] private float _rotationDuration = 0.125f;
        [SerializeField] private bool _offsetRotation = true;
        [SerializeField] private float _offsetAngle = -90;
        //[SerializeField] private float _rotationSpeed = 100f;

        private Quaternion _startRotation;
        private Quaternion _targetRotation;
        private float _angleDifference;
        private float _rotationTimer;
        private float _rotationAnglePerSecond = 45;
        private float _angleIncrement;
        private RotationModes _currentRotationMode;

        #endregion

        #region Life Cycle

        public override void OnStart()
        {
            base.OnStart();
            VisualTransform ??= _renderer ? _renderer.transform : _mainVisualTransform;
        }
        
        public override void OnUpdate()
        {
            UpdateHeading();
            
            if (!IsRotating) return;
            
            switch (_currentRotationMode)
            {
                case RotationModes.TimeBased:
                    TimeBasedRotation();
                    break;
                case RotationModes.IncrementBased:
                    IncrementalRotation();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Public Methods

        public void StopRotating()
        {
            IsRotating = false;
        }

        public void StartRotatingInSetTime(Vector3 direction, float rotationDuration = 0.125f, Action onRotationFinished = null)
        {
            _currentRotationMode = RotationModes.TimeBased;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (_offsetRotation ? _offsetAngle : 0)));

            _startRotation = VisualTransform.rotation;
            _rotationDuration = rotationDuration;
            _rotationTimer = 0;
            IsRotating = true;
            OnRotationFinished = onRotationFinished;
        }
        
        public void StartRotatingIncrementally(Vector3 direction, Action onRotationFinished = null)
        {
            _currentRotationMode = RotationModes.IncrementBased;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + _offsetAngle));

            _startRotation = VisualTransform.rotation;
            _angleDifference = Quaternion.Angle(_targetRotation, _startRotation);
            _angleIncrement = 0;
            IsRotating = true;
            OnRotationFinished = onRotationFinished;
        }

        public void SetColor(Color color)
        {
            _renderer.color = color;
        }

        #endregion

        #region Private Methods

        private void TimeBasedRotation()
        {
            _rotationTimer += Time.deltaTime;
            var t = Mathf.Clamp01(_rotationTimer / _rotationDuration);

            VisualTransform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, t);

            if (t >= 1)
            {
                VisualTransform.rotation = _targetRotation;
                IsRotating = false;
                OnRotationFinished?.Invoke();
            }
        }
        
        private void IncrementalRotation()
        {
            _angleIncrement += _rotationAnglePerSecond * Time.deltaTime;
            var t = Mathf.Clamp01(_angleIncrement / _angleDifference);
            
            VisualTransform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, t);
        
            if (t >= 1)
            {
                VisualTransform.rotation = _targetRotation;
                IsRotating = false;
                OnRotationFinished?.Invoke();
            }
        }
        
        private void UpdateHeading()
        {
            var angle = VisualTransform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            Heading = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        }

        #endregion
    }
}