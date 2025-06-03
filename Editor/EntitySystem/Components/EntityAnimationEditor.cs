using SpookyCore.Runtime.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityAnimation))]
    public class EntityAnimationEditor : EntityComponentEditor
    {
        private EntityAnimation _entityAnimation;
        private string _requiredPath = "Visual/MainVisual";
        private Sprite _defaultSprite;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!_defaultSprite)
            {
                _defaultSprite = GetASpriteFromConfig();
            }
            
            _entityAnimation = (EntityAnimation)target;
            
            CheckAssignReferencesOnEnable(_requiredPath);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!_entityAnimation)
            {
                return;
            }
            
            if (!_entityAnimation.AnimationConfig)
            {
                EditorGUILayout.HelpBox("Please assign a config derived from EntityAnimationConfig.", MessageType.Error);
            }

            if (_entityAnimation.AnimationConfig && _entityAnimation.AnimationConfig.ID != _entity.ID)
            {
                EditorGUILayout.HelpBox("EntityID mismatch between Entity and EntityAnimationConfig.", MessageType.Error);
            }
            
            if (!_entityAnimation._animatorController)
            {
                EditorGUILayout.HelpBox("Please assign an Animator Controller.", MessageType.Error);
                if (GUILayout.Button("Open Animator Controller Builder"))
                {
                    AnimatorControllerBuilderWindow.OpenWindow((EntityAnimation)target);
                }
            }
            
            CheckBuildingHierarchyOnInspectorGUI(_requiredPath, "Build Visual Hierarchy");
        }

        protected override void AssignReferences(Transform downMostTransform)
        {
            var animator = _entity.gameObject.GetComponent<Animator>();
            var spriteRenderer = downMostTransform.GetComponent<SpriteRenderer>();
            if (!_defaultSprite)
            {
                _defaultSprite = GetASpriteFromConfig();
            }
            
            if (_entityAnimation._animatorController &&
                _entityAnimation.AnimationConfig && 
                animator.runtimeAnimatorController &&
                spriteRenderer && spriteRenderer.sprite != _defaultSprite)
            {
                return;
            }

            if (!animator.runtimeAnimatorController)
            {
                animator.runtimeAnimatorController = _entityAnimation._animatorController;
            }
            
            if (spriteRenderer && spriteRenderer.sprite != _defaultSprite)
            {
                spriteRenderer.sprite = _defaultSprite;
            }
            
            EditorUtility.SetDirty(_entity.gameObject);
            AssetDatabase.SaveAssets();
        }
        
        private Sprite GetASpriteFromConfig()
        {
            if (!_entityAnimation || !_entityAnimation.AnimationConfig)
            {
                return null;
            }
            var firstClip = _entityAnimation.AnimationConfig.AnimationClips[0];
            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(firstClip);

            foreach (var binding in bindings)
            {
                if (binding.propertyName != "m_Sprite") continue;

                var keyframes = AnimationUtility.GetObjectReferenceCurve(firstClip, binding);
                if (keyframes.Length > 0 && keyframes[0].value is Sprite sprite)
                {
                    return sprite;
                }
            }

            return null;
        }
    }
}