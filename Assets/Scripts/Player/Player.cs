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
    [SerializeField] private Transform _itemPos;
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
        set
        {
            _heldPickable = value;
            SetupItem(value);
        }
    }
    public Room CurrentRoom { get => _currentRoom; set => _currentRoom = value; }
    public bool Selected { get => _selected; set => _selected = value; }
    public Transform UI => _UI;
    private bool _selected;

    public int Index
    {
        get;
        set;
    }

    private void Start()
    {
        CurrentRoom = GameManager.Instance.Hub;
    }

    private void SetupItem(PickableData item)
    {
        if (item == null)
        {
            for (int i = 0; i < _itemPos.childCount; i++)
            {
                Destroy(_itemPos.GetChild(i).gameObject);
            }
        }
        else
        {
            GameObject obj = Instantiate(item.Prefab, _itemPos);
            
            obj.transform.localPosition = Vector3.zero;
            Destroy(obj.GetComponent<Item>());
            Destroy(obj.transform.GetChild(0).gameObject);
        }
    }
}

public enum HubRelativePosition
{
    HUB,
    LEFT_WING,
    RIGHT_WING
}
