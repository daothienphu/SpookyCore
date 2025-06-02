using System;
using System.Collections.Generic;
using System.Linq;

namespace SpookyCore.Runtime.Systems
{
    public static class BootstrapSort
    {
        public static List<Type> TopoSort(List<Type> systemTypes)
        {
            var sorted = new List<Type>();
            var visited = new HashSet<Type>();
            var visiting = new HashSet<Type>();

            foreach (var type in systemTypes)
            {
                Visit(type, sorted, visited, visiting);
            }

            return sorted;
        }

        private static void Visit(Type type, List<Type> sorted, HashSet<Type> visited, HashSet<Type> visiting)
        {
            if (visited.Contains(type)) return;
            if (visiting.Contains(type)) throw new Exception("Cycle detected");

            visiting.Add(type);

            foreach (var dep in GetDependencies(type))
            {
                Visit(dep, sorted, visited, visiting);
            }

            visiting.Remove(type);
            visited.Add(type);
            sorted.Add(type);
        }

        public static List<Type> GetDependencies(Type type)
        {
            return type.GetCustomAttributes(typeof(DependsOnAttribute), true)
                    .Cast<DependsOnAttribute>()
                    .Select(a => a.DependencyType)
                    .ToList();
        }
    }
}