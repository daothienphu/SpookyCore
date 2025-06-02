using SpookyCore.Runtime.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityInputReceiver), true)]
    public class EntityInputReceiverEditor : EntityComponentEditor
    {
        private EntityInputReceiver _inputReceiver;

        protected override void OnEnable()
        {
            base.OnEnable();
            _inputReceiver = (EntityInputReceiver)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var useNewInputSystem = serializedObject.FindProperty("_useNewInputSystem")?.boolValue;
            
            //check for IInputManager
            if (useNewInputSystem.HasValue && useNewInputSystem.Value && true)
            {
                EditorGUILayout.HelpBox("There is no InputManager that implements the New Input System in the scene.", MessageType.Error);
                if (GUILayout.Button("Create InputManager"))
                {
                    Debug.Log("Created InputManager");
                    //todo: handle creating input manager
                }
            }
        }
    }
}