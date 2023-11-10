using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{

    [FormerlySerializedAs("_heldItem")] [SerializeField] private PickableData _heldPickable;
    [SerializeField] private PlayerController _playerController;
    public PlayerController PlayerController => _playerController;
    
    [SerializeField] private HubRelativePosition _relativePos;
    public HubRelativePosition RelativePos { get => _relativePos; set => _relativePos = value; }
    

    [SerializeField]
    private Room _currentRoom;

    public PickableData HeldPickable
    {
        get => _heldPickable;
        set => _heldPickable = value;
    }
    public Room CurrentRoom { get => _currentRoom; set => _currentRoom = value; }

    public int Index => _playerController.PlayerIndex;
}

public enum HubRelativePosition
{
    HUB,
    LEFT_WING,
    RIGHT_WING
}
