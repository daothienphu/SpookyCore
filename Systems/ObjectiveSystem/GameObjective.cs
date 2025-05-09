using System;
using System.Collections.Generic;
using SpookyCore.EntitySystem;
using SpookyCore.SystemLoader;
using SpookyCore.Utilities.Events;
using UnityEngine;

namespace SpookyCore.Systems.ObjectiveSystem
{
    [CreateAssetMenu(menuName = "SpookyCore/Game Objectives/Objective", fileName = "Game_Objective")]
    public class GameObjective : ScriptableObject
    {
        #region Structs & Enums

        [Serializable]
        public enum ObjectiveID
        {
            None,
            M_Tutorial_1,
            M_Test_2,
            M_Test_3,
            S_Test_1,
            S_Test_2,
        }
        
        [Serializable]
        public enum ObjectiveAction
        {
            None,
            Collect, //Items
            Capture, //Fishes
            Kill, //Fishes
            Upgrade, //Bots, Weapons, Armors
            Discover, //Waypoints
            Unlock, //No idea
        }
        
        [Serializable]
        public enum ObjectiveType
        {
            None,
            MainObjective,
            SideObjective,
        }

        [Serializable]
        public enum ObjectiveState
        {
            Inactive, 
            Active, 
            Completed,
        }

        [Serializable]
        public struct ObjectiveRewardData
        {
            public GameItemSO Item;
            public int Amount;
        }
        
        [Serializable]
        public class ObjectiveRequirementData
        {
            public ObjectiveAction Action;
            public EntityID Target;
            public int RequiredAmount;
            public string VerboseDescription;

            public int ProgressAmount;
            public bool IsCompleted;

            public void UpdateProgress(int progress)
            {
                ProgressAmount = progress;
                if (ProgressAmount == RequiredAmount)
                {
                    IsCompleted = true;
                }
            }
        }

        #endregion

        #region Fields

        public SavableGameObjectiveData Data;
        public ObjectiveType Type;
        public string ObjectiveName;
        [TextArea(15, 20)] 
        public string Description;
        public List<ObjectiveRequirementData> Requirements;
        public List<GameObjective> PrerequisiteObjectives;
        public List<ObjectiveRewardData> ItemRewards;
        public float CurrentProgress;

        private IEventBus _eventBus;
        
        #endregion

        #region Public Methods

        public void UpdateProgress(ObjectiveAction action, EntityID target)
        {
            CurrentProgress = 0;
            foreach (var requirement in Requirements)
            {
                if (requirement.IsCompleted)
                {
                    CurrentProgress++;
                    continue;
                }

                if (requirement.Action != action || requirement.Target != target)
                {
                    continue;
                }
                
                requirement.UpdateProgress(requirement.ProgressAmount + 1);

                _eventBus ??= ServiceLocator.Instance.Get<IEventBus>();
                _eventBus.Publish(new ObjectiveUpdateEvent(requirement.VerboseDescription, requirement.ProgressAmount));
                CurrentProgress += (float)requirement.ProgressAmount / requirement.RequiredAmount;
            }
            
            CurrentProgress /= Requirements.Count;
            
            if (Mathf.Approximately(CurrentProgress, 1))
            {
                Data.State = ObjectiveState.Completed;
            }
        }
        
        public bool HasIncompleteActionRequirement(ObjectiveAction action)
        {
            foreach (var requirement in Requirements)
            {
                if (requirement.IsCompleted)
                {
                    continue;
                }
                
                if (requirement.Action == action)
                {
                    return true;
                }
            }

            return false;
        }

        public List<ObjectiveAction> GetActions()
        {
            var actions = new List<ObjectiveAction>();
            foreach (var requirement in Requirements)
            {
                actions.Add(requirement.Action);
            }
            return actions;
        }

        public List<EntityID> GetTargets()
        {
            var targets = new List<EntityID>();
            foreach (var requirement in Requirements)
            {
                targets.Add(requirement.Target);
            }
            return targets;
        }

        public SavableGameObjectiveData SaveObjectiveData()
        {
            var data = new SavableGameObjectiveData
            {
                ID = Data.ID,
                State = Data.State,
                RequirementProgress = new List<int>()
            };
            
            foreach (var req in Requirements)
            {
                data.RequirementProgress.Add(req.ProgressAmount);
            }

            return data;
        }
        
        public void LoadObjectiveProgress(SavableGameObjectiveData data)
        {
            Data.State = data.State;
            Data.RequirementProgress = data.RequirementProgress;

            for (var i = 0; i < Data.RequirementProgress.Count; i++)
            {
                var req = Data.RequirementProgress[i];
                Requirements[i].UpdateProgress(req);
            }
        }

        #endregion
    }
}