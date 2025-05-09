using UnityEngine;

namespace SpookyCore.Utilities.Editor.Attributes
{
    public class ShowIfTrueAttribute : PropertyAttribute
    {
        public string ConditionFieldName { get; private set; }

        public ShowIfTrueAttribute(string conditionFieldName)
        {
            ConditionFieldName = conditionFieldName;
        }
    }
}