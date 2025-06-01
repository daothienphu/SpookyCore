using System.Collections.Generic;
using SpookyCore.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SpookyCore.SystemLoader
{
    public class SystemsLoader : MonoSingleton<SystemsLoader>
    {
        [SerializeField] private List<GameSystemLoader> _systems;
        //[SerializeField] private float _loadingProgress;
        //[SerializeField] private Slider _loadingSlider;
        [SerializeField] private bool _switchToTestUI;

        private void Start()
        {
            for (var i = 0; i < _systems.Count; i++)
            {
                var system = _systems[i];
                InstantiateSystem(system.SystemPrefab);
                // if (_systems.Count != 0)
                // {
                //     _loadingProgress = 1.0f * (i + 1) / _systems.Count;
                // }
            }
            
            DoneLoadingSystem();
        }

        // private void Update()
        // {
        //     _loadingSlider.value = _loadingProgress;
        // }

        private void InstantiateSystem(GameObject prefab)
        {
            Instantiate(prefab);
        }

        private void DoneLoadingSystem()
        {
            if (_switchToTestUI)
            {
                GameSessionData.Instance.SwitchToUITest();
            }
            else
            {
                GameSessionData.Instance.SwitchToGameplay();
            }
        }
    }
}