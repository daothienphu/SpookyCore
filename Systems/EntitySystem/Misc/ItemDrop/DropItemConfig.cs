using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpookyCore.EntitySystem
{
    [CreateAssetMenu(menuName = "SpookyCore/Game Item Configs/Drop Item Config", fileName = "Drop_Item_Config")]
    public class DropItemConfig : ScriptableObject
    {
        [Serializable]
        public class RarityDropChance
        {
            public ItemRarity Rarity;
            [Range(0, 1)] public float Chance;
        }
        
        [field: SerializeField] public int MinItemDropCount;
        [field: SerializeField] public int MaxItemDropCount;
        [field: SerializeField] public ItemRarity MinItemRarity;
        [field: SerializeField] public ItemRarity MaxItemRarity;
        [field: SerializeField] public List<GameItemSO> PossibleDrops;
        [field: SerializeField] public List<RarityDropChance> DropChances;

        public float GetDropChance(ItemRarity rarity)
        {
            foreach (var entry in DropChances)
            {
                if (entry.Rarity == rarity)
                {
                    return entry.Chance;
                }
            }
            return 0;
        }
    }

    [Serializable]
    public enum ItemRarity
    {
        Common = 0,
        Rare = 1,
        Exceptional = 2,
    }
}