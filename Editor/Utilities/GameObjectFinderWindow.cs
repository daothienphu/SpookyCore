using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Utilities.Editor
{
    public class GameObjectFinderWindow : EditorWindow
    {
        private string _selectedTag = "Untagged"; // Default tag
        private LayerMask _selectedLayer; // Default layer (Default layer is 0)
        private readonly List<GameObject> _foundObjects = new();
        private Vector2 _scrollPosition;
        private bool _layerOnly;
        private bool _tagOnly;
        
        [MenuItem("SpookyTools/Utilities/GameObject Finder")]
        public static void ShowWindow()
        {
            GetWindow<GameObjectFinderWindow>("GameObject Finder");
        }

        private void OnGUI()
        {
            GUILayout.Label("Find GameObjects by Layer or Tag", EditorStyles.boldLabel);
            
            _selectedLayer = EditorGUILayout.LayerField("Select Layer", _selectedLayer);
            _selectedTag = EditorGUILayout.TagField("Select Tag", _selectedTag);
            _layerOnly = EditorGUILayout.Toggle("Layer Only", _layerOnly);
            _tagOnly = EditorGUILayout.Toggle("Tag Only", _tagOnly);

            if (GUILayout.Button("Find GameObjects"))
            {
                FindGameObjects();
            }

            GUILayout.Space(10);
            GUILayout.Label($"Found {_foundObjects.Count} GameObjects", EditorStyles.boldLabel);
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(200));
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
                if ((_tagOnly && obj.CompareTag(_selectedTag)) || 
                    (_layerOnly && obj.layer == _selectedLayer))// ||
                    //(obj.IsInLayer(_selectedLayer) || obj.CompareTag(_selectedTag)))
                {
                    _foundObjects.Add(obj);
                }
            }
            
            _foundObjects.Sort((x, y) => string.Compare(x.name, y.name, StringComparison.Ordinal));
        }
    }
}