using System;

namespace SpookyCore.Runtime.Systems
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependsOnAttribute : Attribute
    {
        public Type DependencyType { get; }
        public DependsOnAttribute(Type dependencyType) => DependencyType = dependencyType;
    }
}