using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    private PressurePlateManager _manager;
    public bool IsActive=false;
    [HideInInspector] public UnityEvent OnPressPressurePlate;
    [HideInInspector] public UnityEvent OnLeavePressurePlate;

    private void Awake()
    {
        _manager = GetComponentInParent<PressurePlateManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.GetComponent<Player>() || other.GetComponent<Furniture>()) && !IsActive)
        {
            Debug.Log(IsActive);
            OnPressPressurePlate.Invoke();
            IsActive =true;
            _manager.CheckIfValid();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.GetComponent<Player>() || other.GetComponent<Furniture>())&&IsActive)
        {
            OnLeavePressurePlate.Invoke();
            IsActive=false;
            _manager.CheckIfValid();
        }
    }
}
