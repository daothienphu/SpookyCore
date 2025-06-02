using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public static class AnimationStateUtility
    {
        private static readonly Dictionary<EntityAnimState, string> StateToString = new();
        private static readonly Dictionary<(string, EntityID), EntityAnimState> StringToState = new();
            
        /// <summary>
        /// Converts an AnimationState to a string, format: Entity_State -> "State"
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string ToAnimationString(this EntityAnimState state)
        {
            if (StateToString.TryGetValue(state, out var result))
            {
                return result;
            }
            
            result = state.ToString();
            for (var i = 0; i < result.Length; i++)
            {
                var c = result[i];
                if (c == '_')
                {
                    return result.Substring(i + 1, result.Length - i - 1);
                }
            }
            
            StateToString.Add(state, result);

            return result;
        }

        /// <summary>
        /// Converts a string to an AnimationState with respect to an EntityID, format: "State" -> Entity_State.
        /// Only used in the AnimationStateEnumGenerator script.
        /// </summary>
        /// <param name="stringState"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EntityAnimState ToAnimationState(this string stringState, EntityID id)
        {
            if (StringToState.TryGetValue((stringState, id), out var result))
            {
                return result;
            }
            
            Enum.TryParse($"{id.ToString()}_{stringState}", out result);
            
            StringToState.Add((stringState, id), result);
            
            return result;
        }
    }
}