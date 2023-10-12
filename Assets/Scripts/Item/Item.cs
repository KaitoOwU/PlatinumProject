using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
    [Header("--- DATA ---")]
    private Collider _collider;
    [SerializeField] private ItemData _itemData;
    [SerializeField] private List<Player> _playersInRange = new();

    [Header("--- EVENTS ---")]
    [SerializeField] private UnityEvent _onPlayerEnterRange;
    [SerializeField] private UnityEvent _onPlayerExitRange;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        Player p = other.GetComponent<Player>();
        _playersInRange.Add(p);

        p.PlayerController.Inputs.OnInteract.AddListener(() => PlayerInteracted(p));
            
        _onPlayerEnterRange?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() == null)
            return;
        
        Player p = other.GetComponent<Player>();
        _playersInRange.Remove(p);
        
        p.PlayerController.Inputs.OnInteract.RemoveListener(() => PlayerInteracted(p));
        
        _onPlayerExitRange?.Invoke();
    }

    private void PlayerInteracted(Player player)
    {
        player.HoldedItem = _itemData;
        Destroy(gameObject);
    }
}
