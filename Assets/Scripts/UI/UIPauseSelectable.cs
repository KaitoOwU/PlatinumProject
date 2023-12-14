using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPauseSelectable : MonoBehaviour, ISelectHandler
{
    private UIPauseMenu _pauseMenu;

    private void Awake()
    {
        _pauseMenu = GetComponentInParent<UIPauseMenu>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        _pauseMenu.SelectButton(gameObject);
    }
}
