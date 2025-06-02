// using SpookyCore.Runtime.EntitySystem;
// using SpookyCore.SystemLoader;
//
// namespace SpookyCore.Runtime.Systems
// {
//     public class InventoryUpdateEvent : GameEventContext
//     {
//         public EntityID ItemID;
//         public int AmountChange;
//
//         public InventoryUpdateEvent(EntityID itemID, int amountChange)
//         {
//             ItemID = itemID;
//             AmountChange = amountChange;
//         }
//
//         public InventoryUpdateEvent Override(EntityID itemID, int amountChange)
//         {
//             ItemID = itemID;
//             AmountChange = amountChange;
//             return this;
//         }
//     }
// }