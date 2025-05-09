using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace SpookyCore.Utilities
{
    public static class Extensions
    {
        #region Collections

        public static bool TryAdd<T>(this List<T> list, T element)
        {
            if (!list.Contains(element))
            {
                list.Add(element);
                return true;
            }
            return false;
        }

        public static bool TryRemove<T>(this List<T> list, T element)
        {
            if (!list.Contains(element))
            {
                return false;
            }
            list.Remove(element);
            return true;
        }

        public static bool TryFindIndexOf<T>(this List<T> list, T item, out int index)
        {
            if (!list.Contains(item))
            {
                index = -1;
                return false;
            }
            index = list.IndexOf(item);
            return true;
        }
        
        

        #endregion

        #region Vector

        /// <summary>
        /// Returns the square distance between a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float SqrDistance(this Vector3 a, Vector3 b)
        {
            return Vector3.SqrMagnitude(a - b);
        }

        /// <summary>
        /// Converts a Vector3 to a Vector2 using the x and y coordinates.
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector2 V2(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        /// <summary>
        /// Converts a Vector2 to a Vector3.
        /// </summary>
        /// <param name="v2"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 V3(this Vector2 v2, float z = 0)
        {
            return new Vector3(v2.x, v2.y, z);
        }

        #endregion

        #region Transform & GameObject

        /// <summary>
        /// Deletes all children of a transform.
        /// </summary>
        /// <param name="t"></param>
        public static void DeleteAllChildren(this Transform t)
        {
            foreach (Transform child in t)
            {
                Object.Destroy(child.gameObject);
            }
        }

        public static bool IsInLayer(this GameObject go, int layer)
        {
            return ((1 << go.layer) & (1 << layer)) != 0;
        }
        
        public static bool IsInLayer(this GameObject go, LayerMask referenceLayerMask)
        {
            return ((1 << go.layer) & referenceLayerMask) != 0;
        }
        
        public static bool IsInLayer(this Transform transform, int layer)
        {
            return IsInLayer(transform.gameObject, layer);
        }
        
        public static bool IsInLayer(this Transform transform, LayerMask referenceLayerMask)
        {
            return IsInLayer(transform.gameObject, referenceLayerMask);
        }

        public static bool IsInLayer(this Transform transform, string layer)
        {
            var layerMask = LayerMask.NameToLayer(layer);
            return IsInLayer(transform, layerMask);
        }

        #endregion

        #region String

        public static string Bold(this string str) => "<b>" + str + "</b>";
        public static string Color(this string str, string color) => $"<color={color}>{str}</color>";
        public static string Italic(this string str) => "<i>" + str + "</i>";
        public static string Size(this string str, int size) => $"<size={size}>{str}</size>";

        #endregion

        #region UI
        
        private static PointerEventData _eventDataCurrentPosition;
        private static List<RaycastResult> _results;
        
        /// <summary>
        /// Returns the world position with respect to the main camera.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Vector2 GetWorldPosition(this RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, GameCache.Camera, out var result);
            return result;
        }

        /// <summary>
        /// Returns true if the pointer is currently over this UI element.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool IsPointerOverThisUI(this GameObject gameObject)
        {
            _eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            _results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
            foreach (var raycastResult in _results)
            {
                if (raycastResult.gameObject == gameObject)
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Returns true if the pointer is currently over any UI element.
        /// </summary>
        /// <returns></returns>
        public static bool IsPointerOverAnyUI()
        {
            _eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            _results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
            return _results.Count > 0;
        }

        #endregion
    }
}
