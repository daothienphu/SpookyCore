using UnityEditor;
using UnityEngine;

namespace SpookyCore.Utilities
{
    [CustomEditor(typeof(WaterSurface))]
    public class WaterSurfaceEditor : UnityEditor.Editor
    {
        private WaterSurface _waterSurface;
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!_waterSurface)
            {
                _waterSurface = (WaterSurface)target;
            }

            if (GUILayout.Button("Update Water Params"))
            {
                _waterSurface.UpdateWaterParamsInEditor();
                EditorUtility.SetDirty(_waterSurface);
            }
            
            if (GUILayout.Button("Generate Mesh"))
            {
                _waterSurface.PreviewMeshInEditor();
                EditorUtility.SetDirty(_waterSurface);
            }
        }
    }
}