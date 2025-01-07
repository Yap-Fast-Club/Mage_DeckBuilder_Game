using System;
using System.Collections.Generic;
using System.Linq;
using NueGames.NueDeck.Scripts.Data.Containers;
using NueGames.NueDeck.Scripts.Enums;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Managers
{
    public class AudioManager : MonoBehaviour
    {
        private AudioManager(){}
        public static AudioManager Instance { get; private set; }

        [SerializeField]private AudioSource musicSource;
        [SerializeField]private AudioSource sfxSource;
        [SerializeField]private AudioSource buttonSource;

        private float _ogMusicVolume;
        private float _ogSFXVolume;
        
        [SerializeField] private List<SoundProfileData> soundProfileDataList;
        
        private Dictionary<AudioActionType, SoundProfileData> _audioDict = new Dictionary<AudioActionType, SoundProfileData>();
        private float lastSoundTime = 0f;
        private Queue<AudioActionType> soundQueue = new Queue<AudioActionType>();


        #region Setup
        private void Awake()
        {

            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                _ogMusicVolume = musicSource.volume;
                _ogSFXVolume = sfxSource.volume;
                transform.parent = null;
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                for (int i = 0; i < Enum.GetValues(typeof(AudioActionType)).Length; i++)
                    _audioDict.Add((AudioActionType)i,soundProfileDataList.FirstOrDefault(x=>x.AudioType == (AudioActionType)i));
            }

        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
                musicSource.volume = musicSource.volume == 0 ? _ogMusicVolume : 0;

            if (Input.GetKeyDown(KeyCode.S))
                sfxSource.volume = sfxSource.volume == 0 ? _ogSFXVolume : 0;

            CheckAndPlayNextSound();
        }

      
        public void AddToSoundQueue(AudioActionType audioType)
        {

            if ( soundQueue.Where(a => a == audioType).Count() < _audioDict[audioType].SoundQueueLimit)
            {
                soundQueue.Enqueue(audioType);
            }
        }

        private void CheckAndPlayNextSound()
        {
            if (soundQueue.Count > 0 && Time.time - lastSoundTime >= 0.03333333f)
            {
                var nextAudioType = soundQueue.Dequeue();
                var clip = _audioDict[nextAudioType].GetRandomClip();

                PlayOneShot(clip);
                lastSoundTime = Time.time;
            }
        }
        #endregion



        #region Public Methods

        public void PlayMusic(AudioClip clip)
        {
            if (!clip) return;
            
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PlayMusic(AudioActionType type)
        {
            if (type == AudioActionType.None) return;

            var clip = _audioDict[type].GetRandomClip();
            if (clip)
                PlayMusic(clip);
        }

        public void PlayOneShot(AudioActionType type)
        {
            if (type == AudioActionType.None) return;

            AddToSoundQueue(type);
        }

        
        public void PlayOneShotButton(AudioActionType type)
        {
            if (type == AudioActionType.None) return;

            var clip = _audioDict[type].GetRandomClip();
            if (clip)
                PlayOneShotButton(clip);
        }

        public void PlayOneShot(AudioClip clip)
        {
            if (clip)
                sfxSource.PlayOneShot(clip);
        }
        
        public void PlayOneShotButton(AudioClip clip)
        {
            if (clip)
                buttonSource.PlayOneShot(clip);
        }

        #endregion
    }
}
