using System.IO;
using SpookyCore.Runtime.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityVisual), editorForChildClasses: true)]
    public class EntityVisualEditor : EntityComponentEditor
    {
        private EntityVisual _visual;
        private readonly string _requiredPath = "Visual/MainVisual";
        private static string _defaultSpritePath = "";
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            _visual = (EntityVisual)target;
            if (_defaultSpritePath == "")
            {
                _defaultSpritePath = Path.Combine(SpookyPathResolver.GetSpookyCorePath(), "Runtime/EntitySystem/Utils/Visual/DefaultSprite.png");
            }

            CheckAssignReferencesOnEnable(_requiredPath);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!_visual.MainVisualRenderer || !_visual.MainVisualTransform)
            {
                EditorGUILayout.HelpBox("Please assign both MainVisualTransform and MainVisualRenderer.", MessageType.Error);
            }
            
            CheckBuildingHierarchyOnInspectorGUI(_requiredPath, "Build Visual Hierarchy");
        }

        protected override void AssignReferences(Transform downMostTransform)
        {
            if (_visual.MainVisualTransform && _visual.MainVisualRenderer)
            {
                return;
            }
            
            _visual.MainVisualTransform = downMostTransform;
            
            var spriteRenderer = downMostTransform.GetComponent<SpriteRenderer>();
            if (!spriteRenderer)
            {
                spriteRenderer = downMostTransform.gameObject.AddComponent<SpriteRenderer>();
            }

            if (!spriteRenderer.sprite)
            {
                spriteRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(_defaultSpritePath);
            }
            
            _visual.MainVisualRenderer = spriteRenderer;
            EditorUtility.SetDirty(_entity.gameObject);
            AssetDatabase.SaveAssets();
        }
    }
}