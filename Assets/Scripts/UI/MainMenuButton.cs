using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MainMenuButton : MonoBehaviour, ISelectHandler
{
    public Image SelectImg { get; private set; }
    private MainMenuUIManager _manager;

    private void Awake()
    {
        SelectImg = GetComponentInChildren<Image>();
        _manager = GetComponentInParent<MainMenuUIManager>();
        
        SelectImg.transform.localScale = new Vector3(0, 1, 1);
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        _manager.DeselectAll();
        SelectImg.transform.DOScaleX(1, 0.7f).SetEase(Ease.OutExpo);
    }
}
