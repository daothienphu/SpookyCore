using SpookyCore.Runtime.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityEnemyDetector), true)]
    public class EntityEnemyDetectorEditor : EntityComponentEditor
    {
        private EntityEnemyDetector _enemyDetector;
        private readonly string _requiredPath = "Collider/EnemyDetector";

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _enemyDetector = target as EntityEnemyDetector;
            
            CheckAssignReferencesOnEnable(_requiredPath);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!_enemyDetector._colliderListener)
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
            
            EditorGUILayout.Toggle("Has Detected", _enemyDetector.HasDetectedEnemies);
            
            var firstDetected = _enemyDetector.FirstDetectedEnemy;
            EditorGUILayout.ObjectField("First Detected Entity", 
                firstDetected ? firstDetected.gameObject : null, 
                typeof(GameObject), true);

            var closestDetected = _enemyDetector.ClosestDetectedEnemy;
            EditorGUILayout.ObjectField("Closest Detected Entity", 
                closestDetected ? closestDetected.gameObject : null,
                typeof(GameObject), true);
            
            EditorGUILayout.LabelField("All Detected Entities:");
            EditorGUI.indentLevel++;
            foreach (var entity in _enemyDetector.DetectedEnemies)
            {
                EditorGUILayout.ObjectField(entity.name, entity.gameObject, typeof(GameObject), true);
            }
            EditorGUI.indentLevel--;
        }

        protected override void AssignReferences(Transform downMostTransform)
        {
            if (_enemyDetector._colliderListener &&
                _enemyDetector._colliderListener.ParentEntityEnemyDetector == _enemyDetector)
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

            listener.ParentEntityEnemyDetector = _enemyDetector;
            _enemyDetector._colliderListener = listener;
            
            EditorUtility.SetDirty(_enemyDetector.gameObject);
            AssetDatabase.SaveAssets();
        }
    }

}