using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{

    [FormerlySerializedAs("_holdedItem")] [SerializeField] private ItemData _heldItem;
    private PlayerController _playerController;

    public PlayerController PlayerController => _playerController;
    public Room CurrentRoom => _currentRoom;

    [SerializeField]
    private Room _currentRoom;

    public ItemData HeldItem
    {
        get => _heldItem;
        set => _heldItem = value;
    }

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }
}
