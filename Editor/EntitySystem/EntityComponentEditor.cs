using SpookyCore.EntitySystem;
using UnityEditor;

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

        private void OnEditorUpdate()
        {
            if (!_component && _entity)
            {
                _entity.RefreshComponentsCache();
                EditorApplication.update -= OnEditorUpdate;
            }
        }
    }
}