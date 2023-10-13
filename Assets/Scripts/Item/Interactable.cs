using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    protected List<Player> _playersInRange = new();
    protected Collider _collider;
    
    [Header("--- EVENTS ---")]
    [SerializeField] protected UnityEvent _onPlayerEnterRange;
    [SerializeField] protected UnityEvent _onPlayerExitRange;
    
    protected void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    
    protected void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        Player p = other.GetComponent<Player>();
        _playersInRange.Add(p);

        p.PlayerController.Inputs.OnInteract.AddListener(() => OnInteract(p));
            
        _onPlayerEnterRange?.Invoke();
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() == null)
            return;
        
        Player p = other.GetComponent<Player>();
        _playersInRange.Remove(p);
        
        p.PlayerController.Inputs.OnInteract.RemoveListener(() => OnInteract(p));
        
        _onPlayerExitRange?.Invoke();
    }

    protected virtual void OnInteract(Player player){}
    
    protected virtual void PlayAnimation(){}

}
