using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using SpookyCore.Runtime.Systems;

namespace SpookyCore.Editor.Systems
{
    public class BootstrapSystemViewer : EditorWindow
    {
        [MenuItem("SpookyTools/Systems/Bootstrap System/Bootstrap Dependencies Viewer")]
        public static void ShowWindow()
        {
            GetWindow<BootstrapSystemViewer>("Bootstrap Dependencies Viewer");
        }

        private Vector2 _scroll;

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IBootstrapSystem).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();

            foreach (var type in types)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField(type.Name, EditorStyles.boldLabel);

                var deps = BootstrapSort.GetDependencies(type);
                if (deps.Count == 0)
                {
                    EditorGUILayout.LabelField("Dependencies: None");
                }
                else
                {
                    EditorGUILayout.LabelField("Depends on:");
                    foreach (var d in deps)
                        EditorGUILayout.LabelField(" - " + d.Name);
                }

                if (type.GetCustomAttributes(typeof(PrefabSystemAttribute), true).FirstOrDefault() is PrefabSystemAttribute prefabAttr)
                {
                    EditorGUILayout.LabelField("Prefab:", prefabAttr.ResourcePath);
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
