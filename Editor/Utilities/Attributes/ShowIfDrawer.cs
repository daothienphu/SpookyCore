using UnityEditor;
using UnityEngine;

namespace SpookyCore.Utilities.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(ShowIfTrueAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var showIf = (ShowIfTrueAttribute)attribute;
            var conditionProperty = property.serializedObject.FindProperty(
                property.propertyPath.Replace(property.name, showIf.ConditionFieldName)
            );
            if (conditionProperty is { propertyType: SerializedPropertyType.Boolean })
            {
                if (conditionProperty.boolValue)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            else
            {
                Debug.LogWarning($"Condition field '{showIf.ConditionFieldName}' not found or not a boolean.");
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var showIf = (ShowIfTrueAttribute)attribute;
            var conditionProperty = property.serializedObject.FindProperty(
                property.propertyPath.Replace(property.name, showIf.ConditionFieldName)
            );
            if (conditionProperty is { propertyType: SerializedPropertyType.Boolean })
            {
                return conditionProperty.boolValue ? EditorGUI.GetPropertyHeight(property, label, true) : 0f;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}