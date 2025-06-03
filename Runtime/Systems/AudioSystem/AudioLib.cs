using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    [CreateAssetMenu(menuName = "SpookyCore/Systems/Audio System/Audio Library", fileName = "Audio_Library")]
    public class AudioLib : ScriptableObject
    {
        [Serializable]
        public enum AudioID
        {
            SFX_Sample,
            Music_Sample,
            Ambient_Sample,
        }
        
        [Serializable]
        public class AudioEntry
        {
            public AudioID ID;
            public AudioClip Clip;
        }

        public AudioClip GetClipByEnum(AudioID id)
        {
            var clip = AudioEntries.Find(e => e.ID == id);
            if (clip == null)
            {
                Debug.Log($"Can't find audio with ID: {id}");
            }

            return clip?.Clip;
        }
        
        [field: SerializeField] public List<AudioEntry> AudioEntries = new();
    }
}