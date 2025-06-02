using UnityEngine;

namespace SpookyCore.Runtime.Utilities
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