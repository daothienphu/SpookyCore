// using System.Collections.Generic;
// using SpookyCore.SystemLoader;
// using SpookyCore.EntitySystem;
// using SpookyCore.Utilities;
// using UnityEngine;
//
// namespace SpookyCore.Systems.ObjectiveSystem
// {
//     public class ObjectiveSystem : MonoSingleton<ObjectiveSystem>
//     {
//         #region Fields
//
//         public List<GameObjective> GameObjectives = new();
//         private Dictionary<GameObjective.ObjectiveID, GameObjective> _objectivesByID = new();
//         // private Dictionary<GameObjective.ObjectiveAction, List<GameObjective>> _objectiveByAction = new();
//         private Dictionary<EntityID, List<GameObjective>> _objectivesByTarget = new();
//         private Dictionary<GameObjective.ObjectiveType, List<GameObjective>> _objectiveByType = new();
//         private List<GameObjective> _trackedObjectives = new();
//         private IEventBus _eventBus;
//         private GameSessionData _gameSessionData;
//         
//         #endregion
//
//         #region Life Cycle
//
//         protected override void OnStart()
//         {
//             _eventBus = ServiceLocator.Instance.Get<IEventBus>();
//             _gameSessionData = GameSessionData.Instance;
//             
//             CacheObjectiveDictionaries();
//             Debug.Log("<color=green>[Objectives]</color> system ready.");
//         }
//
//         #endregion
//         
//         #region Public Methods
//
//         public List<SavableGameObjectiveData> SaveObjectiveProgress()
//         {
//             var data = new List<SavableGameObjectiveData>();
//             _gameSessionData.GameData.PlayerData.Objectives.Clear();
//             foreach (var objective in GameObjectives)
//             {
//                 data.Add(objective.SaveObjectiveData());
//             }
//
//             return data;
//         }
//         
//         public void LoadObjectiveProgress(List<SavableGameObjectiveData> objectiveData)
//         {
//             foreach (var data in objectiveData)
//             {
//                 if (_objectivesByID.TryGetValue(data.ID, out var objective))
//                 {
//                     objective.LoadObjectiveProgress(data);
//                 }
//             }
//         }
//         
//         public GameObjective GetObjectiveByID(GameObjective.ObjectiveID id)
//         {
//             if (_objectivesByID.TryGetValue(id, out var objective))
//             {
//                 return objective;
//             }
//             
//             Debug.Log($"Can't find objective with ID: {id}");
//             return null;
//         }
//         
//         public List<GameObjective> GetObjectivesByType(GameObjective.ObjectiveType type)
//         {
//             return _objectiveByType[type];
//         }
//
//         public GameObjective.ObjectiveID GetFirstActiveObjective()
//         {
//             foreach (var objective in GameObjectives)
//             {
//                 if (objective.Data.State == GameObjective.ObjectiveState.Active)
//                 {
//                     return objective.Data.ID;
//                 }
//             }
//             
//             return default;
//         }
//         
//         public void UpdateProgress(GameObjective.ObjectiveAction action, EntityID target)
//         {
//             if (! _objectivesByTarget.TryGetValue(target, out var objectives)) return;
//
//             foreach (var objective in objectives)
//             {
//                 if (objective.Data.State != GameObjective.ObjectiveState.Active)
//                 {
//                     continue;
//                 }
//                 
//                 if (objective.HasIncompleteActionRequirement(action))
//                 {
//                     objective.UpdateProgress(action, target);
//                 }
//             }
//         }
//
//         #endregion
//
//         #region Private Methods
//
//         private void CacheObjectiveDictionaries()
//         {
//             foreach (var objective in GameObjectives)
//             {
//                 //ObjectivesByID
//                 if (_objectivesByID.TryGetValue(objective.Data.ID, out var value))
//                 {
//                     Debug.Log($"Objective ID collision: {value.ObjectiveName}, {objective.ObjectiveName}");
//                 }
//                 else
//                 {
//                     _objectivesByID.Add(objective.Data.ID, objective);
//                 }
//                 
//                 // //ObjectivesByActions
//                 // var actions = objective.GetActions();
//                 // foreach (var action in actions)
//                 // {
//                 //     if (_objectiveByAction.TryGetValue(action, out var objectivesWithAction))
//                 //     {
//                 //         objectivesWithAction.Add(objective);
//                 //     }
//                 //     else
//                 //     {
//                 //         _objectiveByAction.Add(action, new List<GameObjective>{objective});
//                 //     }
//                 // }
//                 
//                 //ObjectivesByTarget
//                 var targets = objective.GetTargets();
//                 foreach (var target in targets)
//                 {
//                     if (_objectivesByTarget.TryGetValue(target, out var objectivesWithTarget))
//                     {
//                         objectivesWithTarget.Add(objective);
//                     }
//                     else
//                     {
//                         _objectivesByTarget.Add(target, new List<GameObjective>{objective});
//                     }
//                 }
//                 
//                 //ObjectivesByType
//                 if (_objectiveByType.TryGetValue(objective.Type, out var objectivesWithType))
//                 {
//                     objectivesWithType.Add(objective);
//                 }
//                 else
//                 {
//                     _objectiveByType.Add(objective.Type, new List<GameObjective>{objective});
//                 }
//             }
//         }
//
//         #endregion
//     }
// }