using System;
using SpookyCore.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityComponent), editorForChildClasses: true)]
    public class EntityComponentEditor : UnityEditor.Editor
    {
        protected Entity _entity;
        private EntityComponent _component;
        
        protected virtual void OnEnable()
        {
            _component = (EntityComponent)target;
            _entity = _component.gameObject.GetComponent<Entity>();

            if (_entity)
            {
                EditorApplication.update += OnEditorUpdate;
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        #region For updating inspector

        protected virtual void OnEditorUpdate()
        {
            if (!_component && _entity)
            {
                _entity.RefreshComponentsCache();
                EditorApplication.update -= OnEditorUpdate;
            }
        }

        #endregion

        #region For building hierarchy

        protected virtual void CheckAssignReferencesOnEnable(string requiredPath)
        {
            if (!NeedsBuildingHierarchy(_entity.transform, requiredPath, out var downMostTransform))
            {
                AssignReferences(downMostTransform);
            }
        }
        
        protected virtual void CheckBuildingHierarchyOnInspectorGUI(string requiredPath, string buttonLabel)
        {
            if (NeedsBuildingHierarchy(_entity.transform, requiredPath, out _))
            {
                if (GUILayout.Button(buttonLabel))
                {
                    CreateHierarchy(_entity.transform, requiredPath, AssignReferences);
                }
            }
        }
        
        protected virtual bool NeedsBuildingHierarchy(Transform root, string fullPath, out Transform downMostTransform)
        {
            fullPath = fullPath.Trim('/');
            var pathComponents = fullPath.Split('/');

            foreach (var pathComponent in pathComponents)
            {
                var child = root.Find(pathComponent);
                if (!child)
                {
                    downMostTransform = null;
                    return true;
                }
                root = child;
            }

            downMostTransform = root;
            return false;
        }

        protected virtual void CreateHierarchy(Transform root, string path, Action<Transform> onPathCreated)
        {
            path = path.Trim('/');
            var pathTree = path.Split('/');

            Undo.SetCurrentGroupName("Setup Hierarchy Tree");
            var group = Undo.GetCurrentGroup();
            foreach (var pathComponent in pathTree)
            {
                var child = root.Find(pathComponent);
                if (!child)
                {
                    var go = new GameObject(pathComponent);
                    Undo.RegisterCreatedObjectUndo(go, "Create " + pathComponent);
                    child = go.transform;
                    child.SetParent(root);
                    child.localPosition = Vector3.zero;
                }

                root = child;
            }
            
            Undo.CollapseUndoOperations(group);
            Debug.Log($"<color=cyan>{_component.GetType().Name}</color> of <color=cyan>{_entity.GetType().Name}</color> created the hierarchy <color=cyan>{_entity.name}/{path}</color>");
            
            onPathCreated?.Invoke(root);
        }

        protected virtual void AssignReferences(Transform downMostTransform) { }
        
        #endregion
    }
}