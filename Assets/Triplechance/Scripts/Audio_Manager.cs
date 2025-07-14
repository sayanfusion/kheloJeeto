using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    public static Audio_Manager instance;
    public AudioSource AudioSource;
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

    public void StopAudio()
    {
        AudioSource.loop = false;
        AudioSource.Stop();
    }

}
