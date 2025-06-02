using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    [CreateAssetMenu(fileName = "SceneFlowConfig", menuName = "SpookyCore/Systems/Scene Flow System/Scene Flow Config")]
    public class SceneFlowConfig : ScriptableObject
    {
        [Serializable]
        public struct SceneMapping
        {
            public SceneID Scene;
            public int BuildIndex;
        }

        [SerializeField] private List<SceneMapping> _sceneMappings = new();

        private Dictionary<SceneID, int> _indexLookup;

        private void OnEnable()
        {
            _indexLookup = new Dictionary<SceneID, int>();
            foreach (var mapping in _sceneMappings)
                _indexLookup[mapping.Scene] = mapping.BuildIndex;
        }

        public int GetSceneBuildIndex(SceneID scene)
        {
            if (_indexLookup.TryGetValue(scene, out var index))
                return index;

            Debug.LogError($"Scene {scene} not found in SceneFlowConfig.");
            return -1;
        }
    }
}