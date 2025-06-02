using System;

namespace SpookyCore.Runtime.Systems
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PrefabSystemAttribute : Attribute
    {
        public string ResourcePath { get; }
        public PrefabSystemAttribute(string resourcePath) => ResourcePath = resourcePath;
    }
}