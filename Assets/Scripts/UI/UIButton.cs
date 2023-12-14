using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, ISelectHandler
{
    public UnityEvent OnButtonSelect;
    
    public void OnSelect(BaseEventData eventData)
    {
        OnButtonSelect?.Invoke();
    }
}