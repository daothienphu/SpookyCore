using UnityEditor;
using UnityEngine;

namespace SpookyCore.Utilities
{
    [CustomEditor(typeof(FOVVisualizer))]
    public class FOVVisualizerEditor : UnityEditor.Editor
    {
        private FOVVisualizer _fov;
        
        private void OnSceneGUI()
        {
            if (!_fov)
            {
                _fov = (FOVVisualizer)target;
            }

            Handles.color = Color.yellow;
            Handles.DrawWireArc(_fov.transform.position, Vector3.forward, Vector3.right, 360, _fov._viewRadius);
            var viewAngleA = _fov.AngleToDirection(- _fov._viewAngle / 2, false);
            var viewAngleB = _fov.AngleToDirection(_fov._viewAngle / 2, false);
            
            Handles.DrawLine(_fov.transform.position, _fov.transform.position + viewAngleA * _fov._viewRadius);
            Handles.DrawLine(_fov.transform.position, _fov.transform.position + viewAngleB * _fov._viewRadius);
            
            Handles.color = Color.green;
            foreach (var visibleTarget in _fov._visibleTargets)
            {
                Handles.DrawLine(_fov.transform.position, visibleTarget.position);
            }
        }
    }
}