using SpookyCore.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityAnimationConfig), true)]
    public class EntityAnimationConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.LabelField("The first clip will be the Default animation state.");
            if (GUILayout.Button("Open Animation Clip Previewer"))
            {
                AnimationClipPreviewer.ShowWindow();
            }

            if (GUILayout.Button("Generate EntityAnimState"))
            {
                EntityAnimStateGenerator.GenerateEnum();
            }
        }
    }
}