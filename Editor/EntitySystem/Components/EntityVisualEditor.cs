using SpookyCore.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityVisual), editorForChildClasses: true)]
    public class EntityVisualEditor : EntityComponentEditor
    {
        private EntityVisual _visual;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            _visual = (EntityVisual)target;
            
            AddVisual();
        }

        private void AddVisual()
        {
            if (!_entity)
            {
                return;
            }

            var visual = _entity.transform.Find("Visual");
            var mainVisual = visual ? visual.Find("MainVisual") : null;
            
            if (visual && mainVisual)
            {
                _visual.VisualTransform = mainVisual;
                return;
            }
            
            CreateVisualHierarchy(visual, mainVisual);
        }

        private void CreateVisualHierarchy(Transform visual, Transform mainVisual)
        {
            Undo.SetCurrentGroupName("Setup EntityVisual Hierarchy");
            var group = Undo.GetCurrentGroup();
            
            if (!visual)
            {
                var visualGO = new GameObject("Visual");
                Undo.RegisterCreatedObjectUndo(visualGO, "Create Visual");
                visual = visualGO.transform;
                visual.SetParent(_entity.transform);
                visual.localPosition = Vector3.zero;
            }
            
            if (!mainVisual)
            {
                var mainVisualGO = new GameObject("MainVisual");
                Undo.RegisterCreatedObjectUndo(mainVisualGO, "Create MainVisual");
                mainVisual = mainVisualGO.transform;
                mainVisual.SetParent(visual);
                mainVisual.localPosition = Vector3.zero;
            }

            Undo.CollapseUndoOperations(group);

            Debug.Log($"EntityVisual of {_entity.name} created hierarchy:\n{_entity.name}\n└── {visual.name}\n    └── {mainVisual.name}", mainVisual);
        }
    }
}