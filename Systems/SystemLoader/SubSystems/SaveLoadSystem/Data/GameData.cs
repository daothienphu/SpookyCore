using System;

namespace SpookyCore.SystemLoader
{
    [Serializable]
    public class GameData
    {
        public PlayerData PlayerData;
        public WorldData WorldData;

        public GameData()
        {
            PlayerData = new PlayerData();
            WorldData = new WorldData();
        }
        
        public GameData(PlayerData playerData, WorldData worldData)
        {
            PlayerData = playerData;
            WorldData = worldData;
        }
    }
}