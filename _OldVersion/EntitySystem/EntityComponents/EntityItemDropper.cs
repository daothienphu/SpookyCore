// using System;
// using System.Collections.Generic;
// using System.Linq;
// using SpookyCore.Utilities;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// namespace SpookyCore.EntitySystem
// {
//     public class EntityItemDropper : EntityComponent
//     {
//         #region Fields
//
//         [SerializeField] private DropItemConfig _dropItemConfig;
//         [SerializeField] private GameObject _itemPrefab;
//         [field: SerializeField] public List<GameItemSO> DroppedItems { get; set; }
//         private Dictionary<ItemRarity, List<GameItemSO>> _filteredItemsCache = new();
//         private List<float> _cumulativeDropChances = new();
//         private Dictionary<ItemRarity, float> _rarityChanceCache = new(); 
//
//         #endregion
//
//         #region Life Cycle
//
//         public override void OnStart()
//         {
//             base.OnStart();
//             Init();
//         }
//
//         #endregion
//         
//         #region Public Methods
//
//         public List<GameItemSO> DropItems()
//         {
//             if (!_dropItemConfig)
//             {
//                 Debug.LogError($"Drop item config is null on {Entity.gameObject.name}");
//                 return null;
//             }
//
//             var itemCount = Random.Range(_dropItemConfig.MinItemDropCount, _dropItemConfig.MaxItemDropCount + 1);
//             DroppedItems = GenerateDrops(itemCount);
//             return DroppedItems;
//         }
//
//         #endregion
//
//         #region Private Methods
//
//         private List<GameItemSO> GenerateDrops(int itemCount)
//         {
//             var drops = new List<GameItemSO>();
//             
//             for (var i = 0; i < itemCount; i++)
//             {
//                 var selectedItemData = GetRandomItem();
//                 if (selectedItemData)
//                 {
//                     drops.Add(selectedItemData);
//                     if (_itemPrefab)
//                     {
//                         var spawnPosition = Random.insideUnitCircle * 2 + transform.position.V2();
//                         var item = Instantiate(_itemPrefab, spawnPosition, Quaternion.identity).GetComponent<IGameItem>();
//                         item.AddGameItemData(selectedItemData);
//                     }
//                 }
//             }
//
//             return drops;
//         }
//
//         private GameItemSO GetRandomItem()
//         {
//             if (_filteredItemsCache.Count == 0)
//             {
//                 return null;
//             }
//
//             var randomValue = Random.value;
//
//             foreach (var dropChance in _cumulativeDropChances)
//             {
//                 if (randomValue <= dropChance)
//                 {
//                     var rarity = _rarityChanceCache.First(x => Mathf.Abs(x.Value - dropChance) < 0.001f).Key;
//                     var itemsOfRarity = _filteredItemsCache[rarity];
//                     if (itemsOfRarity.Count > 0)
//                     {
//                         return itemsOfRarity[Random.Range(0, itemsOfRarity.Count)];
//                     }
//
//                     break;
//                 }
//             }
//
//             return null;
//         }
//
//         private void Init()
//         {
//             var rarities = ((ItemRarity[])Enum.GetValues(typeof(ItemRarity))).ToList();
//             for (var i = 0; i < rarities.Count; i++)
//             {
//                 var dropChance = _dropItemConfig.GetDropChance(rarities[i]);
//                 _cumulativeDropChances.Add(dropChance + (i == 0 ? 0 : _cumulativeDropChances[i - 1]));
//                 _rarityChanceCache.Add(rarities[i], _cumulativeDropChances[i]);
//                 
//                 
//                 if (!_filteredItemsCache.ContainsKey(rarities[i]))
//                 {
//                     _filteredItemsCache.Add(rarities[i], new List<GameItemSO>());
//                 }
//
//                 foreach (var item in _dropItemConfig.PossibleDrops)
//                 {
//                     if (item.Rarity == rarities[i])
//                     {
//                         _filteredItemsCache[rarities[i]].Add(item);
//                     }
//                 }
//             }
//         }
//
//         #endregion
//     }
// }