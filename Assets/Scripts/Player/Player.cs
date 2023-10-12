using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [SerializeField] private ItemData _holdedItem;
    private PlayerController _playerController;

    public PlayerController PlayerController => _playerController;

    public ItemData HoldedItem
    {
        get => _holdedItem;
        set => _holdedItem = value;
    }

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }
}
