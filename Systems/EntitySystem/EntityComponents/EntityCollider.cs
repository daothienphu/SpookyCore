// using UnityEngine;
//
// namespace SpookyCore.EntitySystem
// {
//     public class EntityCollider : EntityComponent
//     {
//         #region Fields
//
//         [SerializeField] protected BodyCollider _colliderScript;
//         [field: SerializeField] public Collider2D Collider;
//
//         #endregion
//         
//         #region Life Cycle
//
//         public override void OnStart()
//         {
//             base.OnStart();
//             ToggleCollider(true);
//             Entity.OnEntityStateChanged += OnEntityStateChanged;
//         }
//
//         private void OnDisable()
//         {
//             if (Entity)
//             {
//                 Entity.OnEntityStateChanged -= OnEntityStateChanged;
//             }
//         }
//
//         #endregion
//
//         #region Public Methods
//
//         public bool HasCollidedWithSomething()
//         {
//             return _colliderScript?.HasCollidedWithSomething ?? false;
//         }
//         
//         public void ToggleCollider(bool isEnabled)
//         {
//             Collider.enabled = isEnabled;
//         }
//
//         #endregion
//
//         #region Private Methods
//
//         private void OnEntityStateChanged(EntityStateEvent ctx)
//         {
//             switch (ctx.NewState)
//             {
//                 case EntitySystem.Entity.EntityState.Dead:
//                     ToggleCollider(false);
//                     break;
//                 case EntitySystem.Entity.EntityState.Alive:
//                 case EntitySystem.Entity.EntityState.Spawned:
//                     ToggleCollider(true);
//                     break;
//             }
//         }
//
//         #endregion
//     }
// }