﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using SpookyCore.Runtime.EntitySystem;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(Entity), true)]
    public class EntityEditor : UnityEditor.Editor
    {
        private Entity _entity;
        private List<Type> _allComponentTypes;
        private List<Type> _availableComponentTypes;
        private string[] _typeDisplayNames;
        private int _selectedIndex;

        private void OnEnable()
        {
            _entity = (Entity)target;

            _allComponentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    typeof(EntityComponent).IsAssignableFrom(t))
                .ToList();
            
            _entity.RefreshComponentsCache();
            RefreshAvailableComponentList();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (_entity.ID == EntityID.MISSING_ID || _entity.ID.ToString().Contains("________"))
            {
                EditorGUILayout.HelpBox("Please assign an ID for this entity.", MessageType.Error);
                if (GUILayout.Button("Open EntityID Editor"))
                {
                    EntityIDEditorWindow.ShowWindow();
                }
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Quick Add Entity Components", EditorStyles.boldLabel);
            
            RefreshAvailableComponentList();

            if (_availableComponentTypes.Count == 0)
            {
                EditorGUILayout.HelpBox("All available components have already been added.", MessageType.Info);
                return;
            }

            _selectedIndex = EditorGUILayout.Popup("Select Component", _selectedIndex, _typeDisplayNames);

            if (_selectedIndex == 0)
            {
                EditorGUILayout.HelpBox("Please select a component to add.", MessageType.Info);
                return;
            }
            
            if (GUILayout.Button("Add Component"))
            {
                var selectedType = _availableComponentTypes[_selectedIndex - 1];
                
                if (_entity.GetComponent(selectedType))
                {
                    Debug.LogWarning($"Entity {_entity.ID} already has a component of type {selectedType.Name}");
                }
                else
                {
                    Undo.AddComponent(_entity.gameObject, selectedType);
                    _entity.RefreshComponentsCache();
                    RefreshAvailableComponentList();
                }
            }
        }

        private void RefreshAvailableComponentList()
        {
            var attachedTypes = _entity
                .GetAllComponents()
                .Select(c => c.GetType())
                .ToHashSet();

            _availableComponentTypes = _allComponentTypes
                .Where(t => !attachedTypes.Contains(t))
                .OrderBy(t => t.Name)
                .ToList();

            var typeNames = _availableComponentTypes
                .Select(t => ObjectNames.NicifyVariableName(t.Name)).ToList();
            typeNames.Add("");
            for (var i = typeNames.Count - 1; i > 0; --i)
            {
                typeNames[i] = typeNames[i - 1];
            }
            typeNames[0] = "None";
            
            _typeDisplayNames = typeNames.ToArray();

            if (_selectedIndex >= _typeDisplayNames.Length)
            {
                _selectedIndex = 0;
            }
        }
    }
}