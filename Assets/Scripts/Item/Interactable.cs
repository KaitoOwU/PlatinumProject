using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    protected List<Player> _playersInRange = new();
    protected Collider _collider;
    public IReadOnlyList<Player> PlayersInRange => _playersInRange;
    
    [Header("--- EVENTS ---")]
    [SerializeField] protected UnityEvent _onPlayerEnterRange;
    [SerializeField] protected UnityEvent _onPlayerExitRange;
    
    protected void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    
    protected void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;

        PlayerController p = other.GetComponent<PlayerController>();
        _playersInRange.Add(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);

        p.Inputs.OnInteractStarted.AddListener(OnInteract);
        
        _onPlayerEnterRange?.Invoke();
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;
        
        PlayerController p = other.GetComponent<PlayerController>();
        _playersInRange.Remove(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);
        
        p.Inputs.OnInteractStarted.RemoveListener(OnInteract);
        
        _onPlayerExitRange?.Invoke();
    }

    protected void OnDestroy()
    {
        foreach (Player p in _playersInRange)
        {
            GameManager.Instance.PlayerList[p.Index - 1].PlayerController.Inputs.OnInteractStarted?.RemoveListener(OnInteract);
        }
            
    }

    protected virtual void OnInteract(Player player){}
    
    protected virtual void PlayAnimation(){}

}
