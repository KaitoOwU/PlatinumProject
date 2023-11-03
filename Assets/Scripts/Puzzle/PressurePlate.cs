using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    private PressurePlateManager _manager;
    private int _playerValidated = 0;
    public bool IsValid => _playerValidated >= 1;

    private void Awake()
    {
        _manager = GetComponentInParent<PressurePlateManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            _playerValidated++;
            if(_playerValidated == 1)
                _manager.OnPuzzleUpdate?.Invoke(this);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            _playerValidated--;
        }
    }
}
