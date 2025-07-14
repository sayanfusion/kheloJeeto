﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Chiptri : InputDetection
{

    Button button;
    GameObject gClickedImage;
	private Image thisImage;

	public Sprite sNormalState;
	public Sprite sSelectedState;
    private bool isTap;
    private static event System.Action deselectitem;

	private void OnValidate()
	{

	}
	private void Awake()
    {
        thisImage = GetComponent<Image>();
        gClickedImage = transform.GetChild(0).gameObject;
        button = GetComponent<Button>();
		button.onClick.AddListener(delegate {
			OnClick(true);
		});
	}
    private void OnEnable()
    {
        deselectitem += OnDeselectItem;
    }

    private void OnDisable()
    {
        deselectitem -= OnDeselectItem;
    }
    public void EnableDisable(bool _bValue)
    {
		#if UNITY_ANDROID
        if(thisImage == null)
            thisImage = GetComponent<Image>();

        if (_bValue)
			thisImage.sprite = sSelectedState;

		else
			thisImage.sprite = sNormalState;
		
		if(gClickedImage == null)
		gClickedImage = transform.GetChild(0).gameObject;
        gClickedImage.SetActive(_bValue);
		#endif
    }


	public void OnClick(bool _playSound = true)
    {
        //if (_playSound)
        //    SoundController.instance.PlayAudio(SoundController.ClipType.CHIP);
        GamePlay.instance.DeSelectAllChip();
        
        EnableDisable(true);
        Debug.LogError(gameObject.name);
        GamePlay.instance.SelectChip(this, gameObject.name);
        /* GamePlay.instance.lastSelectedChip = this;

         GamePlay.instance.currentSelectedChip = int.Parse(gameObject.name);
         GamePlay.instance.IsRemoveClicked = false;*/
        deselectitem?.Invoke();
        isTap = true;
        gClickedImage.SetActive(true);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (!isTap)
        {
            gClickedImage.SetActive(true);
        }
            
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (!isTap)
        {
            gClickedImage.SetActive(false);
        }
    }
    private void OnDeselectItem()
    {
        isTap = false;
        gClickedImage.SetActive(false);
    }

}