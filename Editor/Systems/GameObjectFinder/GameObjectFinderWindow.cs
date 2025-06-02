using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.Systems
{
    public class GameObjectFinderWindow : EditorWindow
    {
        private string _selectedTag = "Untagged";
        private LayerMask _selectedLayer;
        private readonly List<GameObject> _foundObjects = new();
        private Vector2 _scrollPosition;
        private bool _findLayer;
        private bool _findTag;
        
        [MenuItem("SpookyTools/Systems/GameObject Finder")]
        public static void ShowWindow()
        {
            GetWindow<GameObjectFinderWindow>("GameObject Finder");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("Find GameObjects by Layer or Tag", EditorStyles.boldLabel);

            _findLayer = EditorGUILayout.Toggle("Search With Layer", _findLayer);
            _findTag = EditorGUILayout.Toggle("Search With Tag", _findTag);

            _selectedLayer = _findLayer 
                ? EditorGUILayout.LayerField("Select Layer", _selectedLayer) 
                : 0;

            _selectedTag = _findTag 
                ? EditorGUILayout.TagField("Select Tag", _selectedTag) 
                : "Untagged";
            
            EditorGUILayout.Space(20);
            if (GUILayout.Button("Find GameObjects"))
            {
                FindGameObjects();
            }
            EditorGUILayout.Space(10);
            GUILayout.Label($"Found {_foundObjects.Count} GameObjects", EditorStyles.boldLabel);
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(400));
            foreach (var obj in _foundObjects)
            {
                if (GUILayout.Button($"{obj.name}"))
                {
                    Selection.activeGameObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
            }
            GUILayout.EndScrollView();
        }

        private void FindGameObjects()
        {
            _foundObjects.Clear();
            var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

            foreach (var obj in allObjects)
            {
                var hasCorrectTag = obj.CompareTag(_selectedTag);
                var hasCorrectLayer = obj.layer == _selectedLayer;

                if (_findTag && _findLayer)
                {
                    if (hasCorrectTag && hasCorrectLayer)
                    {
                        _foundObjects.Add(obj);
                    }
                    continue;
                }
                
                if ((_findTag && hasCorrectTag) || 
                    (_findLayer && hasCorrectLayer))
                {
                    _foundObjects.Add(obj);
                }
            }
            
            _foundObjects.Sort((x, y) => string.Compare(x.name, y.name, StringComparison.Ordinal));
        }
    }
}