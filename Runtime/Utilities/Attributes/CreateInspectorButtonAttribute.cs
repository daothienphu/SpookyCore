using System;
using UnityEngine;

namespace SpookyCore.Runtime.Utilities
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