using System;
using System.Collections.Generic;
using SpookyCore.EntitySystem;
using SpookyCore.Systems.ObjectiveSystem;

namespace SpookyCore.SystemLoader
{
    [Serializable]
    public class PlayerData
    {
        public List<SavableGameItemData> Inventory = new();
        public List<SavableGameItemData> Equipment = new();
        public List<SavableGameObjectiveData> Objectives = new();
    }
}