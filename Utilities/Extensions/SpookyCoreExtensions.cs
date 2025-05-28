using System;
using SpookyCore.EntitySystem;
using UnityEngine;

namespace SpookyCore.Utilities
{
    public static class SpookyCoreExtensions
    {
        #region Structs & Enums

        [Flags]
        public enum ColliderTarget
        {
            None = 0,
            Player = 1 << 0,
            AnyEntitiesExceptPlayer = 1 << 1,
            AnyEntities = 1 << 2,
            Waypoints = 1 << 3,
            Ground = 1 <<4,
        }

        #endregion
        
        #region Entity

        public static bool TryGetEntity<T>(this Collision2D collision, out T entityBase) where T : Entity
        {
            var transform = collision.transform;
            transform.TryGetEntity(out var entity);
            if (entity is T tEntity)
            {
                entityBase = tEntity;
                return true;
            }
            entityBase = null;
            return false;
        }
        
        public static bool TryGetEntity<T>(this Collider2D collider, out T entityBase) where T : Entity
        {
            var transform = collider.transform;
            transform.TryGetEntity(out var entity);
            if (entity is T tEntity)
            {
                entityBase = tEntity;
                return true;
            }
            entityBase = null;
            return false;
        }
        
        /// <summary>
        /// Try getting the EntityBase component from a collider.
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool TryGetEntity(this Collider2D collider, out Entity entity)
        {
            var transform = collider.transform;
            return transform.TryGetEntity(out entity);
        }
        
        public static bool TryGetEntity(this Collision2D collision, out Entity entity)
        {
            var transform = collision.transform;
            return transform.TryGetEntity(out entity);
        }

        public static bool TryGetEntity(this Transform transform, out Entity entity)
        {
            // if (!transform.CompareTag(GameTags.EntityCollider))
            // {
            //     entityBase = null;
            //     return false;
            // }
            return transform.TryGetComponent(out entity) ||
                   (transform.parent != null && transform.parent.TryGetComponent(out entity)) ||
                   (transform.parent.parent != null && transform.parent.parent.TryGetComponent(out entity));
        }

        #endregion

        #region Colliders

        private static readonly int _flockingEntityLayer = LayerMask.NameToLayer(GameLayers.FLOCKINGENTITY);
        private static readonly int _entityLayer = LayerMask.NameToLayer(GameLayers.ENTITY);
        private static readonly int _waypointLayer = LayerMask.NameToLayer(GameLayers.WAYPOINT);
        private static readonly int _groundLayer = LayerMask.NameToLayer(GameLayers.GROUND);
        
        /// <summary>
        /// Calculates the distance between 2 2D colliders of 2 entities. Most accurate with CircleColliders.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="otherEntity"></param>
        /// <returns></returns>
        public static float ColliderDistance(this Entity entity, Entity otherEntity)
        {
            //var collider = entity.Get<EntityCollider>().Collider;
            //var otherCollider = otherEntity.Get<EntityCollider>().Collider;
            //return collider.ColliderDistance(otherCollider);
            return 0;
        }

        /// <summary>
        /// Calculates the distance between 2 2D colliders. Most accurate with CircleColliders.
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="otherCollider"></param>
        /// <returns></returns>
        public static float ColliderDistance(this Collider2D collider, Collider2D otherCollider)
        {
            var a = collider switch
            {
                BoxCollider2D boxCollider => (boxCollider.bounds.size.x + boxCollider.bounds.size.y) / 2f,
                CircleCollider2D circleCollider => circleCollider.radius,
                _ => (collider.bounds.size.x + collider.bounds.size.y) / 2f
            };

            var b = otherCollider switch
            {
                BoxCollider2D boxCollider => (boxCollider.bounds.size.x + boxCollider.bounds.size.y) / 2f,
                CircleCollider2D circleCollider => circleCollider.radius,
                _ => (collider.bounds.size.x + collider.bounds.size.y) / 2f
            };
            
            return a + b;
        }
        
        public static bool IsColliderTarget(this Collider2D collider2D, ColliderTarget referenceTarget)
        {
            var targetTransform = collider2D.transform;
            return IsCollidedTarget(targetTransform, referenceTarget);
        }
        
        public static bool IsCollisionTarget(this Collision2D collision2D, ColliderTarget referenceTarget)
        {
            var targetTransform = collision2D.transform;
            return IsCollidedTarget(targetTransform, referenceTarget);
        }

        public static bool IsCollidedTarget(this Transform target, ColliderTarget referenceTarget)
        {
            var result = false;
            
            var isInEntityLayer = target.IsInLayer(_entityLayer);
            var isInFlockingEntityLayer = target.IsInLayer(_flockingEntityLayer);

            var isTaggedPlayer = target.CompareTag(GameTags.Player);
            var isTaggedIgnoredByTriggers = target.CompareTag(GameTags.IgnoredByTriggers);
            var isUntagged = target.CompareTag(GameTags.Untagged);

            //Debug.Log($"entity {isInEntityLayer}, flocking {isInFlockingEntityLayer}, player {isTaggedPlayer}, ignored {isTaggedIgnoredByTriggers}, untagged {isUntagged}");
            
            if ((referenceTarget & ColliderTarget.Player) != 0)
            {
                result |= isInEntityLayer && isTaggedPlayer;
            }

            if ((referenceTarget & ColliderTarget.AnyEntitiesExceptPlayer) != 0)
            {
                result |= (isInEntityLayer || 
                           isInFlockingEntityLayer) 
                          && 
                          !(isTaggedPlayer ||
                            isTaggedIgnoredByTriggers ||
                            isUntagged);
            }

            if ((referenceTarget & ColliderTarget.AnyEntities) != 0)
            {
                result |= 
                    (isInEntityLayer ||
                     isInFlockingEntityLayer) 
                    && 
                    !(isTaggedIgnoredByTriggers || 
                      isUntagged);
            }

            if ((referenceTarget & ColliderTarget.Waypoints) != 0)
            {
                result |= target.IsInLayer(_waypointLayer);
            }

            if ((referenceTarget & ColliderTarget.Ground) != 0)
            {
                result |= target.IsInLayer(_groundLayer);
            }

            return result;
        }
        
        #endregion

        #region Layer

        public static LayerMask ToLayerMask(this int value)
        {
            return (LayerMask)(1 << value);
        }

        #endregion
    }
}