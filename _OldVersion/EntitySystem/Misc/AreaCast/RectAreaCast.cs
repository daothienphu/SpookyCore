﻿// using System.Collections.Generic;
// using SpookyCore.Utilities;
// using UnityEngine;
//
// namespace SpookyCore.EntitySystem
// {
//     public class RectAreaCast : IAreaCast
//     {
//         private Collider2D[] _results = new Collider2D[20];
//         private ContactFilter2D _filter = new()
//         {
//             useLayerMask = true,
//             layerMask = LayerMask.NameToLayer(GameLayers.ENTITY).ToLayerMask()
//         };
//
//         /// <summary>
//         /// Get all targets within the area cast.
//         /// </summary>
//         /// <param name="center">Center point of the rect.</param>
//         /// <param name="size">Width and height of the rect.</param>
//         /// <param name="orientation">The orientation of the rect.</param>
//         /// <returns></returns>
//         public List<Entity> GetTargets(Vector2 center, Vector2 size, float orientation)
//         {
//             var hitCount = Physics2D.OverlapBox(center, size, orientation, _filter, _results); 
//             
//             List<Entity> targets = new();
//             for (var i = 0; i < hitCount; i++)
//             {
//                 if (_results[i].TryGetEntity(out var entity))
//                 {
//                     targets.Add(entity);
//                 }
//             }
//
//             return targets;
//         }
//
//         /// <summary>
//         /// Get all targets within the area cast.
//         /// </summary>
//         /// <param name="center">Center point of the rect.</param>
//         /// <param name="size">Width and height of the rect.</param>
//         /// <param name="orientation">The orientation of the rect.</param>
//         /// <param name="layerMask">Override the default entity LayerMask</param>
//         /// <returns></returns>
//         public List<Entity> GetTargets(Vector2 center, Vector2 size, float orientation, LayerMask layerMask)
//         {
//             _filter.layerMask = layerMask;
//             var result = GetTargets(center, size, orientation);
//             _filter.layerMask = LayerMask.NameToLayer(GameLayers.ENTITY).ToLayerMask();
//             return result;
//         }
//     }
// }