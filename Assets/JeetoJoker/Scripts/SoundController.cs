using DevCommon;
using System;
using UnityEngine;
namespace JeetoJoker
{
    public class SoundController : Singleton<SoundController>
    {
        public AudioSource audioSource;
        public AudioSource voiceAudioSource;

        public AudioClip placeyourchips;
        public AudioClip lastChance;
        public AudioClip noMorebetPlease;
        public AudioClip unselectSound;
        public AudioClip clickSound;
        public AudioClip coinsound;

        private AudioClip[] allAudioClips;

        public enum SoundType
        {
            ButtonClick = 0,
            CoinClick,
            NumberClick,
            RowClick,
            NoBet,
            PlaceBet
        }

        protected override void Awake()
        {
            base.Awake();
            allAudioClips = new AudioClip[Enum.GetNames(typeof(SoundType)).Length];
        }

        public void PlayOneShot(SoundType soundType, int soundIndex)
        {
            if (allAudioClips[soundIndex] == null)
                allAudioClips[soundIndex] = Resources.Load<AudioClip>($"Audios/{soundType}");
            audioSource.PlayOneShot(allAudioClips[soundIndex]);
        }

        public void PlayAudiojeetojoker(AudioClip selectunselect)
        {
            audioSource.clip = selectunselect;
            audioSource.Play();
        }
        public void playcoinsound(AudioClip coinsoundArg)
        {
            audioSource.clip = coinsoundArg;
            audioSource.Play();
        }


        public void PlayPlaceyourChips()
        {
            voiceAudioSource.Stop();
            voiceAudioSource.clip = placeyourchips;
            voiceAudioSource.Play();
        }
        public void PlayNoMoreBet()
        {
            voiceAudioSource.Stop();
            voiceAudioSource.clip = noMorebetPlease;
            voiceAudioSource.Play();
        }
        public void PlayLastchance()
        {
            voiceAudioSource.Stop();
            voiceAudioSource.clip = lastChance;
            voiceAudioSource.Play();
        }
    }

}