using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.Systems
{
    public class GlobalGizmoEditorWindow : EditorWindow
    {
        private static readonly Dictionary<string, bool> _categoryToggleStates = new();
        private static readonly Dictionary<string, bool> _categoryFoldoutStates = new();
        private Vector2 _scrollPosition;
        
        [MenuItem("SpookyTools/Systems/Global Gizmo Controller/Gizmo Controller")]
        public static void ShowWindow()
        {
            GetWindow<GlobalGizmoEditorWindow>("Gizmo Controller");
        }

        [MenuItem("SpookyTools/Systems/Global Gizmo Controller/Clear Cached Values")]
        public static void ClearCachedValues()
        {
            EditorPrefs.SetString(GlobalGizmoController.EditorPrefsKey, "");
        }

        private void OnGUI()
        {
            GUILayout.Label("Gizmos", EditorStyles.boldLabel);
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            var gizmos = GlobalGizmoController.GetAllGizmos();
            var groupedGizmos = gizmos.GroupBy(kvp => GetCategory(kvp.Key)).OrderBy(group => group.Key);

            foreach (var group in groupedGizmos)
            {
                var category = group.Key;
                _categoryFoldoutStates.TryAdd(category, true);
                _categoryToggleStates.TryAdd(category, true);
                
                
                EditorGUILayout.BeginHorizontal();
                
                GUILayout.Space(10);
                _categoryFoldoutStates[category] = EditorGUILayout.Foldout(_categoryFoldoutStates[category], category, true);
                
                var categoryToggleState = EditorGUILayout.Toggle(_categoryToggleStates[category], GUILayout.Width(20));
                var forceSetStatesInCategory = categoryToggleState != _categoryToggleStates[category];
                _categoryToggleStates[category] = categoryToggleState;

                EditorGUILayout.EndHorizontal();
                
                
                if (_categoryFoldoutStates[category])
                {
                    var totalEnabled = 0;
                    
                    foreach (var kvp in group)
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        GUILayout.Space(35);
                        EditorGUILayout.LabelField(GetShortKey(kvp.Key), GUILayout.ExpandWidth(true));
                        GUILayout.FlexibleSpace();
                        var gizmoState = EditorGUILayout.Toggle(
                            forceSetStatesInCategory ? _categoryToggleStates[category] : kvp.Value,
                            GUILayout.Width(35));
                        
                        if (gizmoState != kvp.Value)
                        {
                            GlobalGizmoController.SetGizmoState(kvp.Key, gizmoState);
                        }

                        if (gizmoState)
                        {
                            totalEnabled++;
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }

                    if (_categoryToggleStates[category] && totalEnabled != group.Count())
                    {
                        _categoryToggleStates[category] = false;
                    }

                    if (!_categoryToggleStates[category] && totalEnabled == group.Count())
                    {
                        _categoryToggleStates[category] = true;
                    }
                }
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private static string GetCategory(string key)
        {
            return key.Contains("/") ? key.Split('/')[0] : "Uncategorized";
        }
        
        private static string GetShortKey(string key)
        {
            return key.Contains("/") ? key.Split('/')[1] : key;
        }
    }
}