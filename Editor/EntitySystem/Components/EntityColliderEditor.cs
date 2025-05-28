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
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            _collider = (EntityCollider)target;

            AddCollider();
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Collision Info", EditorStyles.boldLabel);

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

        private void AddCollider()
        {
            if (!_entity)
            {
                return;
            }

            var collider = _entity.transform.Find("Collider");
            var mainCollider = collider ? collider.Find("MainCollider") : null;

            if (collider && mainCollider)
            {
                InjectColliderListener(mainCollider);
                return;
            }
            
            CreateColliderHierarchy(collider, mainCollider);
        }

        private void CreateColliderHierarchy(Transform collider, Transform mainCollider)
        {
            Undo.SetCurrentGroupName("Setup EntityCollider Hierarchy");
            var group = Undo.GetCurrentGroup();
            
            if (!collider)
            {
                var colliderGO = new GameObject("Collider");
                Undo.RegisterCreatedObjectUndo(colliderGO, "Create Collider");
                collider = colliderGO.transform;
                collider.SetParent(_entity.transform);
                collider.localPosition = Vector3.zero;
            }
            
            if (!mainCollider)
            {
                var mainColliderGO = new GameObject("MainCollider");
                Undo.RegisterCreatedObjectUndo(mainColliderGO, "Create MainCollider");
                mainCollider = mainColliderGO.transform;
                mainCollider.SetParent(collider);
                mainCollider.localPosition = Vector3.zero;
            }

            Undo.CollapseUndoOperations(group);

            Debug.Log($"EntityCollider of {_entity.name} created hierarchy:\n{_entity.name}\n└── {collider.name}\n    └── {mainCollider.name}", mainCollider);
            
            InjectColliderListener(mainCollider);
        }

        private void InjectColliderListener(Transform mainCollider)
        {
            var c2D = mainCollider.GetComponent<Collider2D>();
            if (!c2D)
            {
                c2D = Undo.AddComponent<BoxCollider2D>(mainCollider.gameObject);
            }
            
            var listener = mainCollider.GetComponent<ColliderListener>();
            if (!listener)
            {
                listener = Undo.AddComponent<ColliderListener>(mainCollider.gameObject);
            }

            listener.ParentCollider = (EntityCollider)target;
        }
    }
}