using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Utilities.Editor.Gizmos
{
    public static class GlobalGizmoController
    {
        private static Dictionary<string, bool> _gizmoToggles = new();
        private static Dictionary<string, bool> _savedGizmoStates = new();
        public const string EditorPrefsKey = "GlobalGizmoStates";

        static GlobalGizmoController()
        {
            _gizmoToggles.Clear();
            LoadGizmoStates();
        }

        #region Public Methods

        public static bool GetGizmoState(string key)
        {
            if (!_gizmoToggles.ContainsKey(key) && _savedGizmoStates.TryGetValue(key, out var state))
            {
                _gizmoToggles.Add(key, state);
            }
            else if (!_gizmoToggles.ContainsKey(key) && !_savedGizmoStates.ContainsKey(key))
            {
                _gizmoToggles.Add(key, true);
                SaveGizmoStates();
            }
            return _gizmoToggles[key];
        }

        public static void SetGizmoState(string key, bool state)
        {
            _gizmoToggles[key] = state;
            SaveGizmoStates();
        }
        
        public static Dictionary<string, bool> GetAllGizmos() => _gizmoToggles;

        #endregion
        
        #region Private Methods

        private static void SaveGizmoStates()
        {
            var json = JsonUtility.ToJson(new GizmoStateData(_gizmoToggles));
            EditorPrefs.SetString(EditorPrefsKey, json);
        }
    
        private static void LoadGizmoStates()
        {
            if (!EditorPrefs.HasKey(EditorPrefsKey)) return;
            
            var json = EditorPrefs.GetString(EditorPrefsKey);
            var data = JsonUtility.FromJson<GizmoStateData>(json);
            _savedGizmoStates = data != null ? data.ToDictionary() : new Dictionary<string, bool>();
        }
    
        [Serializable]
        private class GizmoStateData
        {
            public List<string> Keys = new();
            public List<bool> Values = new();

            public GizmoStateData(Dictionary<string, bool> dict)
            {
                foreach (var kvp in dict)
                {
                    Keys.Add(kvp.Key);
                    Values.Add(kvp.Value);
                }
            }

            public Dictionary<string, bool> ToDictionary()
            {
                var dict = new Dictionary<string, bool>();
                for (var i = 0; i < Keys.Count; i++)
                {
                    dict[Keys[i]] = Values[i];
                }
                return dict;
            }
        }

        #endregion
        
    }
}