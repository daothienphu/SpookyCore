using SpookyCore.Runtime.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityTrigger), true)]
    public class EntityEnemyDetectorEditor : EntityComponentEditor
    {
        private EntityTrigger _trigger;
        private readonly string _requiredPath = "Collider/EnemyDetector";

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _trigger = target as EntityTrigger;
            
            CheckAssignReferencesOnEnable(_requiredPath);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!_trigger._colliderListener)
            {
                EditorGUILayout.HelpBox("Please assign a Collider Listener.", MessageType.Error);
            }
            
            CheckBuildingHierarchyOnInspectorGUI(_requiredPath, "Build Collider Hierarchy");
            
            DrawDetectionDetails();
        }

        private void DrawDetectionDetails()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Detection Info (Entities Only)", EditorStyles.boldLabel);
            
            EditorGUILayout.Toggle("Has Detected", _trigger.HasTriggered);
            
            var firstDetected = _trigger.FirstDetectedEntity;
            EditorGUILayout.ObjectField("First Detected Entity", 
                firstDetected ? firstDetected.gameObject : null, 
                typeof(GameObject), true);

            var closestDetected = _trigger.ClosestDetectedEnemy;
            EditorGUILayout.ObjectField("Closest Detected Entity", 
                closestDetected ? closestDetected.gameObject : null,
                typeof(GameObject), true);
            
            EditorGUILayout.LabelField("All Detected Entities:");
            EditorGUI.indentLevel++;
            foreach (var entity in _trigger.DetectedEnemies)
            {
                EditorGUILayout.ObjectField(entity.name, entity.gameObject, typeof(GameObject), true);
            }
            EditorGUI.indentLevel--;
        }

        protected override void AssignReferences(Transform downMostTransform)
        {
            if (_trigger._colliderListener &&
                _trigger._colliderListener.ParentEntityTrigger == _trigger)
            {
                return;
            }
            
            var c2D = downMostTransform.GetComponent<Collider2D>();
            if (!c2D)
            {
                c2D = Undo.AddComponent<CircleCollider2D>(downMostTransform.gameObject);
                c2D.isTrigger = true;
            }
            
            var listener = downMostTransform.GetComponent<ColliderListener>();
            if (!listener)
            {
                listener = Undo.AddComponent<ColliderListener>(downMostTransform.gameObject);
            }

            listener.ParentEntityTrigger = _trigger;
            _trigger._colliderListener = listener;
            
            EditorUtility.SetDirty(_trigger.gameObject);
            AssetDatabase.SaveAssets();
        }
    }

}