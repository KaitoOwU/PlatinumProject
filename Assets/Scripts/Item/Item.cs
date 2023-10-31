using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class Item : Interactable
{
    
    [FormerlySerializedAs("_itemData")]
    [Header("--- DATA ---")]
    [SerializeField] private PickableData _pickableData;
    
    public PickableData PickableData { get => _pickableData; set => _pickableData = value; }

    protected override void OnInteract(Player player)
    {
        if (player.HeldPickable != null)
            return;
        
        player.HeldPickable = _pickableData;
        Destroy(gameObject);
    }
}