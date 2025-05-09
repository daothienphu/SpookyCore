using UnityEngine;

namespace SpookyCore.Utilities.SmallSystems
{
    public static class Randomizer
    {
        public static bool RandomBool(float successChance)
        {
            return Random.Range(1, 100) <= successChance;
        }
    }
}