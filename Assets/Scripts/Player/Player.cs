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
    public bool Selected { get => _selected; set => _selected = value; }
    public bool _selected;

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
