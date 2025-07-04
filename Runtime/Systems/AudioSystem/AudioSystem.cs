using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace SpookyCore.Runtime.Systems
{
    public class AudioSystem: PersistentMonoSingleton<AudioSystem>, IBootstrapSystem
    {
        #region Fields

        [SerializeField] private AudioLib AudioLib;
        [SerializeField] private AudioMixer AudioMixer;
        [SerializeField] private AudioSource MusicSource;
        [SerializeField] private AudioSource AmbientSource;
        [SerializeField] private List<AudioSource> _sfxSources = new();
        [SerializeField] private int SFXPoolSize = 10;
        [SerializeField] [Range(0, 1)] private float DefaultSFXVolume = 1;
        
        #endregion

        #region Life Cycle

        public Task OnBootstrapAsync(BootstrapContext context)
        {
            InitSFXPool();
            Debug.Log("<color=cyan>[Audio System]</color> ready.");
            return Task.CompletedTask;
        }
        
        #endregion

        #region Public Methods
        
        public void PlayMusic(AudioLib.AudioID id)
        {
            var clip = AudioLib.GetClipByEnum(id);
            if (clip)
            {
                MusicSource.clip = clip;
                MusicSource.Play();
            }
        }
        
        public void PlayAmbient(AudioLib.AudioID id)
        {
            var clip = AudioLib.GetClipByEnum(id);
            if (clip)
            {
                AmbientSource.clip = clip;
                AmbientSource.loop = true;
                AmbientSource.Play();
            }
        }
        
        public void PlaySFX(AudioLib.AudioID id, float volume = 1f)
        {
            var clip = AudioLib.GetClipByEnum(id);
            if (clip)
            {
                var availableSource = _sfxSources.Find(s => !s.isPlaying);
                if (availableSource)
                {
                    availableSource.clip = clip;
                    availableSource.volume = volume;
                    availableSource.Play();
                }
            }
        }

        public void PlaySFXWithPitch(AudioLib.AudioID id, float minPitch = 0.9f, float maxPitch = 1.1f)
        {
            var clip = AudioLib.GetClipByEnum(id);
            if (clip)
            {
                var availableSource = _sfxSources.Find(s => !s.isPlaying);
                if (availableSource)
                {
                    availableSource.clip = clip;
                    availableSource.pitch = Random.Range(minPitch, maxPitch);
                    availableSource.volume = DefaultSFXVolume;
                    availableSource.Play();
                }
            }
        }
        
        public void SetMusicVolume(float volume)
        {
            AudioMixer.SetFloat("Volume_Music", ToDecibels(volume));
        }
        
        public void SetAmbientVolume(float volume)
        {
            AudioMixer.SetFloat("Volume_Ambient", ToDecibels(volume));
        }

        public void SetSFXVolume(float volume)
        {
            AudioMixer.SetFloat("Volume_SFX", ToDecibels(volume));
        }
        
        #endregion

        #region Private Methods

        private float ToDecibels(float volume)
        {
            return Mathf.Log10(volume) * 20f;
        }
        
        private void InitSFXPool()
        {
            if (!AudioMixer) return;
            for (var i = 0; i < SFXPoolSize; i++)
            {
                var sfxObject = new GameObject("SFX_Source_" + i);
                var sfxSource = sfxObject.AddComponent<AudioSource>();
                sfxSource.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("SFX")[0];
                sfxSource.playOnAwake = false;
                sfxObject.transform.parent = transform;
                _sfxSources.Add(sfxSource);
            }
        }

        #endregion
    }
}
