using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(AnimationClip), true)]
    public class AnimationClipEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Open Sprite Animation Clip Previewer"))
            {
                AnimationClipPreviewer.ShowWindow(target as AnimationClip);
            }
        }
    }
}