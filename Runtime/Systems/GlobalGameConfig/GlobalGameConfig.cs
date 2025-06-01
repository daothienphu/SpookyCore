using SpookyCore.EntitySystem;
using UnityEngine;

namespace SpookyCore.Utilities.Configs
{
    public class GlobalGameConfig : MonoSingleton<GlobalGameConfig>
    {
        public enum ColorAttitude
        {
            Neutral,
            Positive,
            Negative,
        }
        
        [field: SerializeField] public GameColorsConfig Colors;
        [field: SerializeField] public int DesiredFPS = 60;

        public Color GetColor(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => Colors.CommonItemColor,
                ItemRarity.Rare => Colors.RareItemColor,
                ItemRarity.Exceptional => Colors.ExceptionalItemColor,
                _ => Colors.CommonItemColor
            };
        }

        public Color GetColor(ColorAttitude attitude)
        {
            return attitude switch
            {
                ColorAttitude.Positive => Colors.PositiveColor,
                ColorAttitude.Negative => Colors.NegativeColor,
                _ => Colors.NeutralColor
            };
        }
    }
}