using UnityEditor;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    [CustomEditor(typeof(EntityBase), true), CanEditMultipleObjects]
    public class EntityBaseEditor : Editor
    {
        private EntityBase _entity;

        void OnValidate()
        {
            _entity = (EntityBase)target;

            if (_entity)
            {
                _entity.RefreshComponentsList();
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!_entity)
            {
                _entity = (EntityBase)target;
            }

            if (GUILayout.Button("Refresh Components"))
            {
                _entity.RefreshComponentsList();
            }
        }
    }
}