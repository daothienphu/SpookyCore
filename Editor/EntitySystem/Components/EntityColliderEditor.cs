using SpookyCore.Runtime.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityCollider), editorForChildClasses: true)]
    public class EntityColliderEditor : EntityComponentEditor
    {
        private EntityCollider _entityCollider;
        private readonly string _requiredPath = "Collider/MainCollider";
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            _entityCollider = (EntityCollider)target;

            CheckAssignReferencesOnEnable(_requiredPath);
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (!_entityCollider._colliderListener)
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
            
            EditorGUILayout.Toggle("Has Collided", _entityCollider.HasCollided);
            
            var collided = _entityCollider.CollidedEntity;
            EditorGUILayout.ObjectField("First Collided Entity", collided ? collided.gameObject : null, typeof(GameObject), true);
            
            EditorGUILayout.LabelField("All Collided Entities:");
            EditorGUI.indentLevel++;
            foreach (var entity in _entityCollider.CollidedEntities)
            {
                EditorGUILayout.ObjectField(entity.name, entity.gameObject, typeof(GameObject), true);
            }
            EditorGUI.indentLevel--;
        }

        protected override void AssignReferences(Transform downMostTransform)
        {
            var rb2d = _entity.gameObject.GetComponent<Rigidbody2D>();
            
            if (_entityCollider._colliderListener &&
                _entityCollider._colliderListener.ParentEntityCollider == _entityCollider &&
                _entityCollider.Collider2D &&
                
                rb2d &&
                (!_entityCollider._prepareRigidbody2D ||
                    ((rb2d.constraints & RigidbodyConstraints2D.FreezeRotation) != 0 &&
                    rb2d.gravityScale == 0)))
            {
                return;
            }
            
            if (!rb2d)
            {
                rb2d = _entity.gameObject.AddComponent<Rigidbody2D>();
            }
            
            if (_entityCollider._prepareRigidbody2D)
            {
                rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb2d.gravityScale = 0;
            }
            
            var c2D = downMostTransform.GetComponent<Collider2D>();
            if (!c2D)
            {
                c2D = Undo.AddComponent<BoxCollider2D>(downMostTransform.gameObject);
            }
            
            var listener = downMostTransform.GetComponent<ColliderListener>();
            if (!listener)
            {
                listener = Undo.AddComponent<ColliderListener>(downMostTransform.gameObject);
            }

            listener.ParentEntityCollider = _entityCollider;
            _entityCollider._colliderListener = listener;
            _entityCollider.Collider2D = c2D;
            
            EditorUtility.SetDirty(_entityCollider.gameObject);
            AssetDatabase.SaveAssets();
        }
    }
}