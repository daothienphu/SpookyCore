using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SpookyCore.EntitySystem;

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

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Quick Add Entity Components", EditorStyles.boldLabel);

            RefreshAvailableComponentList();

            if (_availableComponentTypes.Count == 0)
            {
                EditorGUILayout.HelpBox("All available components have already been added.", MessageType.Info);
                return;
            }

            _selectedIndex = EditorGUILayout.Popup("Select Component", _selectedIndex, _typeDisplayNames);

            if (GUILayout.Button("Add Component"))
            {
                var selectedType = _availableComponentTypes[_selectedIndex];
                
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

            _typeDisplayNames = _availableComponentTypes
                .Select(t => ObjectNames.NicifyVariableName(t.Name))
                .ToArray();

            if (_selectedIndex >= _availableComponentTypes.Count)
            {
                _selectedIndex = 0;
            }
        }

        private static string NicifyTypeName(string camelCase)
        {
            return Regex.Replace(camelCase, "(\\B[A-Z])", " $1");
        }
    }
}