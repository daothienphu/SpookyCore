using UnityEditor;
using System.IO;

namespace SpookyCore
{
    public static class SpookyPathResolver
    {
        private const string SpookyAsmdefName = "SpookyCore.asmdef";
        private static string _cachedPath;
        
        public static string GetSpookyCorePath()
        {
            if (!string.IsNullOrEmpty(_cachedPath)) return _cachedPath;
            
            var guids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset " + Path.GetFileNameWithoutExtension(SpookyAsmdefName));
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileName(path) == SpookyAsmdefName)
                {
                    _cachedPath = Path.GetDirectoryName(path);
                    return _cachedPath;
                }
            }

            UnityEngine.Debug.LogError($"Could not find {SpookyAsmdefName}. Ensure it exists in the SpookyCore folder.");
            return null;
        }
    }
}