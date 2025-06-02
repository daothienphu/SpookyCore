// using UnityEngine;
// using UnityEditor;
// using System.Linq;
//
// namespace SpookyCore.Utilities.Editor
// {
//     // Commented because I found out sorting group exists
//     
//     /// This just exposes the Sorting Layer / Order in MeshRenderer since it's there
//     /// but not displayed in the inspector. Getting MeshRenderer to render in front
//     /// of a SpriteRenderer is pretty hard without this.
//     // [CustomEditor(typeof(MeshRenderer))]
//     // public class MeshRendererSortingEditor : UnityEditor.Editor
//     // {
//     //     public override void OnInspectorGUI()
//     //     {
//     //         base.OnInspectorGUI();
//     //
//     //         var renderer = target as MeshRenderer;
//     //
//     //         EditorGUILayout.BeginHorizontal();
//     //         EditorGUI.BeginChangeCheck();
//     //         var newId = DrawSortingLayersPopup(renderer.sortingLayerID);
//     //         if (EditorGUI.EndChangeCheck())
//     //         {
//     //             renderer.sortingLayerID = newId;
//     //         }
//     //
//     //         EditorGUILayout.EndHorizontal();
//     //
//     //         EditorGUILayout.BeginHorizontal();
//     //         EditorGUI.BeginChangeCheck();
//     //         var order = EditorGUILayout.IntField("Sorting Order", renderer.sortingOrder);
//     //         if (EditorGUI.EndChangeCheck())
//     //         {
//     //             renderer.sortingOrder = order;
//     //         }
//     //
//     //         EditorGUILayout.EndHorizontal();
//     //     }
//     //
//     //     private int DrawSortingLayersPopup(int layerID)
//     //     {
//     //         var layers = SortingLayer.layers;
//     //         var names = layers.Select(l => l.name).ToArray();
//     //
//     //         var currentIndex = System.Array.FindIndex(layers, l => l.id == layerID);
//     //         if (currentIndex < 0) currentIndex = 0;
//     //
//     //         var newIndex = EditorGUILayout.Popup("Sorting Layer", currentIndex, names);
//     //
//     //         if (newIndex >= 0 && newIndex < layers.Length)
//     //         {
//     //             return layers[newIndex].id;
//     //         }
//     //
//     //         return layers[0].id;
//     //     }
//     // }
// }