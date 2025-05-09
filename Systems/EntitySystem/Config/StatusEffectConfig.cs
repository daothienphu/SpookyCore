using System;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public abstract class StatusEffectConfig : ScriptableObject
    {
        [Serializable]
        public enum AreaShape { Circle, Rectangle, Cone }
        
        public string EffectName;
        public AreaShape EffectShape;
        public Vector2 EffectSize;
        public float Duration;
        public string Cooldown;
        public float Magnitude;
        private IAreaCast _areaCast;

        public abstract StatusEffectHandlerBase CreateEffectHandler();
        
        public IAreaCast GetAreaCast()
        {
            return _areaCast ??= EffectShape switch
            {
                AreaShape.Circle => new CircleAreaCast(),
                AreaShape.Rectangle => new RectAreaCast(),
                AreaShape.Cone => new ConeAreaCast(),
                _ => new CircleAreaCast()
            };
        }
    }
}