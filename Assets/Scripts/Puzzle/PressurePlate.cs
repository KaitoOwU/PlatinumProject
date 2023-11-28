using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    private PressurePlateManager _manager;
    public bool IsActive=false;

    private void Awake()
    {
        _manager = GetComponentInParent<PressurePlateManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Player>() || other.GetComponent<Furniture>() && !IsActive)
        {
            IsActive=true;
            _manager.CheckIfValid();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() || other.GetComponent<Furniture>())
        {
            IsActive=false;
            _manager.CheckIfValid();
        }
    }
}
