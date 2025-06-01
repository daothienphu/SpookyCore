using System.Collections.Generic;
using SpookyCore.EntitySystem;
using SpookyCore.Utilities;
using UnityEngine;

namespace SpookyCore.SystemLoader.SubSystems.ItemDatabase
{
    public class ItemDatabase : PersistentMonoSingleton<ItemDatabase>, IGameSystem
    {
        public List<GameItemSO> Items;
        
        protected override void OnStart()
        {
            base.OnStart();
            Debug.Log("[Item Database]".Color("cyan") + " system ready.");
        }

        public GameItemSO GetItemById(EntityID id)
        {
            foreach (var item in Items)
            {
                if (item.Data.ItemID == id)
                {
                    return item;
                }
            }

            Debug.Log($"Could not find any item with ID: {id}");
            return null;
        }
    }
}