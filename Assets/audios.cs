using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audios : MonoBehaviour
{

    public AudioSource wheelRotatingSound;
    public AudioSource placeBet;
    public AudioSource noMoreBet;
    public AudioClip[] audioClips;
    

    public static audios Instance;
    // Start is called before the first frame update
    void Start()
    {
        InvokingPlaceBet();
        InvokingNoMoreBet();
        Instance = this;
        wheelRotatingSound = gameObject.AddComponent<AudioSource>();
        placeBet = gameObject.AddComponent<AudioSource>();
        noMoreBet = gameObject.AddComponent<AudioSource>();

        ManageSounds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ManageSounds()
    {
        wheelRotatingSound.clip = audioClips[0];
        placeBet.clip = audioClips[1];
        noMoreBet.clip = audioClips[2];
    }

    public void WheelRotatingSound()
    {
        wheelRotatingSound.Play();
    }
    public void WheelRotatingSoundStop()
    {
        wheelRotatingSound.Stop();
    }
    
    public void PlaceBetSound()
    {
        placeBet.Play();
    }
   /* public void NoMoreBetSound()
    {
        noMoreBet.Play();
        SceneRoulette._Instance.clearButton.interactable = false;
        SceneRoulette._Instance.rebetButton.interactable = false;
        SceneRoulette._Instance.undoButton.interactable = false;
        SceneRoulette._Instance.rollButton.interactable = false;
    }*/
    public void NoMoreBetSoundPlay()
    {
        Debug.Log("NoMoreBetSoundPlay " + noMoreBet.clip.name);
        noMoreBet.Play();
        SceneRoulette._Instance.clearButton.interactable = false;
        SceneRoulette._Instance.rebetButton.interactable = false;
        SceneRoulette._Instance.undoButton.interactable = false;
        SceneRoulette._Instance.rollButton.interactable = false;
    }
    public void InvokingNoMoreBet()
    {
         
     //   Invoke("NoMoreBetSound",58);
        
        
    }
    public void InvokingPlaceBet()
    {
        Invoke("PlaceBetSound", 8);
    }



}
