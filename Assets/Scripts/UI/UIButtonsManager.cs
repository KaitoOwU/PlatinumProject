using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonsManager : MonoBehaviour
{
    [SerializeField] protected GameObject _firstSelected;
    [SerializeField] protected Transform _selector;
    [SerializeField] protected float _timer;
    
    protected UIButton[] _buttons;

    private void Awake()
    {
        _buttons = GetComponentsInChildren<UIButton>();
        StartCoroutine(ActivateButton());
    }

    protected IEnumerator ActivateButton()
    {
        yield return new WaitForSecondsRealtime(_timer);
        
        _selector.position = _firstSelected.transform.position;
        EventSystem.current.SetSelectedGameObject(_firstSelected);
    }
}
