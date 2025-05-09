using System;
using UnityEngine;

namespace SpookyCore.Utilities.Editor.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CreateInspectorButtonAttribute : PropertyAttribute
    {
        public string ButtonLabel { get; }

        public CreateInspectorButtonAttribute(string buttonLabel = null)
        {
            ButtonLabel = buttonLabel;
        }
    }
}