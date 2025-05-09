using UnityEditor;

namespace SpookyCore.EntitySystem
{
    [CustomEditor(typeof(EntityComponentBase), true), CanEditMultipleObjects]
    public class EntityComponentBaseEditor : Editor
    {
        private EntityComponentBase _component;
        private EntityBase _entity;

        private void Awake()
        {
            _component = (EntityComponentBase)target;
            _entity = _component.gameObject.GetComponent<EntityBase>();

            if (_entity)
            {
                _entity.AddComponent(_component);
                EditorApplication.update += OnEditorUpdate;
            }
        }

        private void OnEditorUpdate()
        {
            if (!_component && _entity)
            {
                _entity.RemoveComponent(_component);
                EditorApplication.update -= OnEditorUpdate;
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
