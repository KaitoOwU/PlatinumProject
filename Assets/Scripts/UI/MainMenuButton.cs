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
    private MainMenuUIManager _manager;

    private void Awake()
    {
        _manager = GetComponentInParent<MainMenuUIManager>();
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        _manager.Select(this);
    }
}
