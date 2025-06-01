using SpookyCore.EntitySystem;
using SpookyCore.EntitySystem.Utils;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityCollider), editorForChildClasses: true)]
    public class EntityColliderEditor : EntityComponentEditor
    {
        private EntityCollider _collider;
        private readonly string _requiredPath = "Collider/MainCollider";
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            _collider = (EntityCollider)target;

            CheckAssignReferencesOnEnable(_requiredPath);
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (!_collider._colliderListener)
            {
                EditorGUILayout.HelpBox("Please assign a Collider Listener.", MessageType.Error);
            }
            
            CheckBuildingHierarchyOnInspectorGUI(_requiredPath, "Build Collider Hierarchy");
            
            DrawCollisionDetails();
        }

        private void DrawCollisionDetails()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Collision Info (Entities Only)", EditorStyles.boldLabel);
            
            EditorGUILayout.Toggle("Has Collided", _collider.HasCollided);
            
            var collided = _collider.CollidedEntity;
            EditorGUILayout.ObjectField("First Collided Entity", collided ? collided.gameObject : null, typeof(GameObject), true);
            
            EditorGUILayout.LabelField("All Collided Entities:");
            EditorGUI.indentLevel++;
            foreach (var entity in _collider.CollidedEntities)
            {
                EditorGUILayout.ObjectField(entity.name, entity.gameObject, typeof(GameObject), true);
            }
            EditorGUI.indentLevel--;
        }

        protected override void AssignReferences(Transform downMostTransform)
        {
            var c2D = downMostTransform.GetComponent<Collider2D>();
            if (!c2D)
            {
                Undo.AddComponent<BoxCollider2D>(downMostTransform.gameObject);
            }
            
            var listener = downMostTransform.GetComponent<ColliderListener>();
            if (!listener)
            {
                listener = Undo.AddComponent<ColliderListener>(downMostTransform.gameObject);
            }

            listener.ParentEntityCollider = _collider;
            _collider._colliderListener = listener;
            EditorUtility.SetDirty(_collider.gameObject);
            AssetDatabase.SaveAssets();
        }
    }
}