using SpookyCore.Runtime.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Runtime.AI.BehaviourTree.Editor
{
    [CustomEditor(typeof(EntityAIRunner))]
    public class EntityAIRunnerEditor : UnityEditor.Editor
    {
        private EntityAIRunner _entityAIRunner;
        private SerializedProperty _updateIntervalProperty;
        private SerializedProperty _behaviorsProperty;

        private void OnEnable()
        {
            _behaviorsProperty = serializedObject.FindProperty("Behaviours");
            _updateIntervalProperty = serializedObject.FindProperty("_behaviorTreeUpdateInterval");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            
            if (!_entityAIRunner)
            {
                _entityAIRunner = (EntityAIRunner)target;
            }

            serializedObject.Update();

            EditorGUILayout.PropertyField(_updateIntervalProperty, new GUIContent("Update Interval"), true);
            EditorGUILayout.PropertyField(_behaviorsProperty, new GUIContent("Behaviors"), true);
            
            serializedObject.ApplyModifiedProperties();
            
            if (GUILayout.Button("Show Execution Path"))
            {
                BehaviourTreeEditor.ShowWindow(_entityAIRunner);
            }
        }
    }
}