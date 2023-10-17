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
    

    [SerializeField]
    private Room _currentRoom;

    public ItemData HeldItem
    {
        get => _heldItem;
        set => _heldItem = value;
    }
    public Room CurrentRoom { get => _currentRoom; set => _currentRoom = value; }

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }
}
