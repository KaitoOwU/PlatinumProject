using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{

    [SerializeField] private ItemData _heldItem;

    public ItemData HeldItem
    {
        get => _heldItem;
        set => _heldItem = value;
    }

    public int Index
    {
        get;
        set;
    }
}
