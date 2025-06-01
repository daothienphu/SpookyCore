using UnityEngine;
using System.Collections.Generic;

namespace SpookyCore.Utilities
{
    public static class GameCache
    {
        private static Camera _camera;

        public static Camera Camera
        {
            get
            {
                if (!_camera)
                    _camera = Camera.main;
                return _camera;
            }
        }
        
        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new();

        public static WaitForSeconds WaitForSeconds(float time)
        {
            if (WaitDictionary.TryGetValue(time, out var wait))
            {
                return wait;
            }

            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }
    }
}