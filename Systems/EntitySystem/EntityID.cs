using System;

namespace SpookyCore.EntitySystem
{
    [Serializable]
    public enum EntityID
    {
        MISSING_ID = 0,
        
        Planet = 101,
        
        ___________________________________STRUCTURE = 200,
        Normal_Structure = 201,
        
        ___________________________________ASTEROID = 300,
        Normal_Asteroid = 301,
        
        ___________________________________PROJECTILE = 400,
        Normal_Projectile = 401,
        
        ___________________________________WAYPOINT = 500,
        
        ___________________________________ITEM = 600,
        
        ___________________________________WEAPON = 700,
        
        ___________________________________ARMOR = 800,
    }
    
    public static class ItemTypeChecker
    {
        public static bool IsFish(this EntityID entityID)
        {
            var intValue = (int)entityID;
            return intValue is > 200 and < 300;
        }
        
        public static bool IsFlora(this EntityID entityID)
        {
            var intValue = (int)entityID;
            return intValue is > 300 and < 400;
        }
        
        public static bool IsBot(this EntityID entityID)
        {
            var intValue = (int)entityID;
            return intValue is > 400 and < 500;
        }
        
        public static bool IsWaypoint(this EntityID entityID)
        {
            var intValue = (int)entityID;
            return intValue is > 500 and < 600;
        }
        
        public static bool IsItem(this EntityID entityID)
        {
            var intValue = (int)entityID;
            return intValue is > 600 and < 700;
        }
        
        public static bool IsWeapon(this EntityID entityID)
        {
            var intValue = (int)entityID;
            return intValue is > 700 and < 800;
        }
        
        public static bool IsArmor(this EntityID entityID)
        {
            var intValue = (int)entityID;
            return intValue is > 800 and < 900;
        }
    }
}