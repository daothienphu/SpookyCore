using SpookyCore.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityAnimation))]
    public class EntityAnimationEditor : EntityComponentEditor
    {
        private EntityAnimation _animation;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _animation = target as EntityAnimation;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (!_animation.AnimationConfig)
            {
                EditorGUILayout.HelpBox("Please assign a config derived from EntityAnimationConfig.", MessageType.Error);
            }

            if (_animation.AnimationConfig && _animation.AnimationConfig.ID != _entity.ID)
            {
                EditorGUILayout.HelpBox("EntityID mismatch between Entity and EntityAnimationConfig.", MessageType.Error);
            }
            
            if (!_animation._animatorController)
            {
                EditorGUILayout.HelpBox("Please assign an Animator Controller.", MessageType.Error);
                if (GUILayout.Button("Open Animator Controller Builder"))
                {
                    AnimatorControllerBuilderWindow.OpenWindow((EntityAnimation)target);
                }
            }
        }
    }
}