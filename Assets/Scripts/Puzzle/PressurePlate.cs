using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    private int _count;
    private PressurePlateManager _manager;
    public bool IsActive => _count >= 1;

    private void Awake()
    {
        _manager = GetComponentInParent<PressurePlateManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() || other.GetComponent<Furniture>())
        {
            _count++;
            _manager.CheckIfValid();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() || other.GetComponent<Furniture>())
        {
            _count--;
            _manager.CheckIfValid();
        }
    }
}
