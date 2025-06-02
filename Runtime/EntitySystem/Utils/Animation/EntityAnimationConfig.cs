using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    [CreateAssetMenu(menuName = "SpookyCore/Entity System/Animation/Entity Animation Config", fileName = "EntityAnimation_Config")]
    public class EntityAnimationConfig : ScriptableObject
    {
        public EntityID ID;
        public List<AnimationClip> AnimationClips = new();
    }
}