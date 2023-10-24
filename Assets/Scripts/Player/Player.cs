using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{

    [SerializeField] private ItemData _heldItem;
    [SerializeField] private PlayerController _playerController;
    public PlayerController PlayerController => _playerController;
    
    [SerializeField] private HubRelativePosition _relativePos;
    public HubRelativePosition RelativePos { get; set; }
    

    [SerializeField]
    private Room _currentRoom;

    public ItemData HeldItem
    {
        get => _heldItem;
        set => _heldItem = value;
    }
    public Room CurrentRoom { get => _currentRoom; set => _currentRoom = value; }

    public int Index
    {
        get;
        set;
    }
}

public enum HubRelativePosition
{
    HUB,
    LEFT_WING,
    RIGHT_WING
}
