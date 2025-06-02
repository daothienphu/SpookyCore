using SpookyCore.Runtime.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityMovement), true)]
    public class EntityMovementEditor : EntityComponentEditor
    {
        private EntityMovement _movement;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            _movement = target as EntityMovement;
            
            AssignReferences(_entity.transform);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            AssignReferences(_entity.transform);
        }

        protected override void AssignReferences(Transform downMostTransform)
        {
            var usePlatformerProp = serializedObject.FindProperty("_usePlatformerMovementSet");
            if (usePlatformerProp.boolValue && !_movement.CharacterController)
            {
                if (!downMostTransform.TryGetComponent<SimpleCharacterController2D>(out _))
                {
                    downMostTransform.gameObject.AddComponent<SimpleCharacterController2D>();
                }

                var cc = downMostTransform.GetComponent<SimpleCharacterController2D>();
                _movement.CharacterController = cc;
                
                EditorUtility.SetDirty(_entity.gameObject);
                AssetDatabase.SaveAssets();
            }
        }
    }
}