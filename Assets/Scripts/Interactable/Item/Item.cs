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
    [HideInInspector] public UnityEvent OnPickUpArm;

    public PickableData PickableData { get => _pickableData; set => _pickableData = value; }

    protected override void OnInteract(Player player)
    {
        base.OnInteract(player);

        if (player.HeldPickable != null)
            return;
        OnPickUpArm?.Invoke();
        player.HeldPickable = _pickableData;

        StartCoroutine(_Destroy());
    }
    IEnumerator _Destroy()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}