using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Events;

public class Player : MonoBehaviour
{

    [FormerlySerializedAs("_heldItem")] [SerializeField] private PickableData _heldPickable;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Color _associatedColor, _controllerColor;
    public PlayerController PlayerController => _playerController;
    public Color AssociatedColor => _associatedColor;
    public Color ControllerColor => _controllerColor;

    [SerializeField] private HubRelativePosition _relativePos;
    public HubRelativePosition RelativePos { get => _relativePos; set => _relativePos = value; }


    [SerializeField]
    private Room _currentRoom;

    [SerializeField]
    private Transform _UI;

    [HideInInspector] public UnityEvent OnHit;

    public PickableData HeldPickable
    {
        get => _heldPickable;
        set => _heldPickable = value;
    }
    public Room CurrentRoom { get => _currentRoom; set => _currentRoom = value; }
    public bool Selected { get => _selected; set => _selected = value; }
    public Transform UI => _UI;
    public bool _selected;

    public int Index
    {
        get;
        set;
    }

    private void Start()
    {
        CurrentRoom = GameManager.Instance.Hub;
    }
}

public enum HubRelativePosition
{
    HUB,
    LEFT_WING,
    RIGHT_WING
}
