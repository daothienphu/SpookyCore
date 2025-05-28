// namespace SpookyCore.EntitySystem
// {
//     public class EntityMovement : EntityComponent
//     {
//         public bool CanMove = true;
//         protected EntityData Data;
//         protected EntityVisual Visual;
//
//         public override void OnStart()
//         {
//             Data = Entity.Get<EntityData>();
//             Visual = Entity.Get<EntityVisual>();
//
//         }
//
//         public void ToggleMovement(bool isEnabled)
//         {
//             CanMove = isEnabled;
//         }
//     }
// }