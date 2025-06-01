using UnityEditor;
using UnityEngine;
using System.Reflection;
using SpookyCore.EntitySystem;
using SpookyCore.EntitySystem.Utils.Stat;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityStat), true)]
    public class EntityStatEditor : EntityComponentEditor
    {
        private EntityStat _stat;

        protected override void OnEnable()
        {
            base.OnEnable();
            _stat = (EntityStat)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();

            var statProp = serializedObject.FindProperty("_statConfig");
            if (statProp == null)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }
            
            var stat = statProp.objectReferenceValue as EntityStatConfig;
            if (!stat)
            {
                EditorGUILayout.HelpBox("Please assign a config derived from EntityStatConfig.", MessageType.Error);
            }
            else
            {
                EditorGUILayout.Space(10);
                
                EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawStatList(stat);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStatList(EntityStatConfig config)
        {
            var props = config.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (prop.PropertyType != typeof(Stat)) continue;
                
                if (!prop.CanRead) continue;
                
                if (prop.GetValue(config) is not Stat stat) continue;

                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(prop.Name), EditorStyles.boldLabel);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.FloatField("Base", stat.Base);
                EditorGUILayout.FloatField("Current", stat.Current);
                EditorGUI.EndDisabledGroup();
                //Modifiers here

                EditorGUILayout.EndVertical();
            }
        }
    }
}
