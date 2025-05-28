using System.Globalization;
using UnityEditor;
using UnityEngine;
using SpookyCore.EntitySystem;
using SpookyCore.EntitySystem.Utils.Stat;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityStat), true)]
    public class EntityStatEditor : UnityEditor.Editor
    {
        private EntityStat _stat;
        private SerializedProperty _configProperty;

        private void OnEnable()
        {
            _stat = (EntityStat)target;

            // if (Application.isPlaying)
            // {
            //     _stat.InitializeRuntimeData();
            // }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (!_stat.Stats)
            {
                EditorGUILayout.HelpBox("Please assign a config derived from EntityStatConfig.", MessageType.Error);
            }
            else
            {
                DrawStatList();
            }
        }

        private void DrawStatList()
        {
            var data = _stat.GetData<EntityStatConfig>();
            if (!data)
            {
                EditorGUILayout.HelpBox("Runtime stat data not initialized. Enter Play Mode to see values.",
                    MessageType.Info);
                return;
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Runtime Stats", EditorStyles.boldLabel);

            foreach (var stat in data.GetAllStats())
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField(stat.GetType().Name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Base Value", stat.BaseValue.ToString(CultureInfo.InvariantCulture));
                EditorGUILayout.LabelField("Current Value", stat.CurrentValue.ToString(CultureInfo.InvariantCulture));
                //EditorGUILayout.LabelField("Modifiers", stat.ModifiersCount.ToString());
                EditorGUILayout.EndVertical();
            }
        }
    }
}