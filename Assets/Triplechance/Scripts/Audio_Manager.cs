using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    public static Audio_Manager instance;
    public AudioSource AudioSource;

    public AudioSource clickAudioSource;

    public AudioSource placeChipsSource;

    public AudioSource lastChanceSource;
    private void Awake()
    {
        instance = this;
    }


    public void PlayAudio(AudioClip audioClip)
    {
        if(audioClip.name== "Spinning wheel sound")
        {
            AudioSource.loop = true;
        }
        else
        {
            AudioSource.loop = false;
        }
        AudioSource.clip = audioClip;
        AudioSource.Play();
    }

    public void PlayCLickAudio() {

        Debug.Log("clicked audio called");
        if (clickAudioSource.isPlaying) clickAudioSource.Stop();
        clickAudioSource.Play();


    }

    public void PLayPlaceYourChips() { 
        
        if(placeChipsSource.isPlaying) placeChipsSource.Stop();
        placeChipsSource.Play();

    }

    public void PLayLastChance() { 
    
        if(lastChanceSource.isPlaying) lastChanceSource.Stop();
        lastChanceSource.Play();

    }
    public void StopAudio()
    {
        AudioSource.loop = false;
        AudioSource.Stop();
    }

}
