using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Item : Interactable
{
    
    [Header("--- DATA ---")]
    [SerializeField] private ItemData _itemData;

    protected override void OnInteract(Player player)
    {
        if (player.HeldItem != null)
            return;
        
        player.HeldItem = _itemData;
        Destroy(gameObject);
    }
    
}