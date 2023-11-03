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

        if (p.Inputs == null)
            return;
        
        _playersInRange.Add(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);

        p.Inputs.OnInteract.AddListener(OnInteract);
        p.Inputs.OnPush.AddListener(OnPush);
        p.Inputs.OnPushCanceled.AddListener(OnPushCanceled);
        
        _onPlayerEnterRange?.Invoke();
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;
        
        PlayerController p = other.GetComponent<PlayerController>();
        
        if (p.Inputs == null)
            return;
        
        _playersInRange.Remove(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);
        
        p.Inputs.OnInteract?.RemoveListener(OnInteract);
        p.Inputs.OnPush?.RemoveListener(OnPush);
        p.Inputs.OnPushCanceled?.RemoveListener(OnPushCanceled);


        _onPlayerExitRange?.Invoke();
    }

    protected void OnDestroy()
    {
        foreach (Player p in _playersInRange)
        {
            GameManager.Instance.PlayerList[p.Index - 1].PlayerController.Inputs.OnInteract?.RemoveListener(OnInteract);
            GameManager.Instance.PlayerList[p.Index - 1].PlayerController.Inputs.OnPush?.RemoveListener(OnPush);
            GameManager.Instance.PlayerList[p.Index - 1].PlayerController.Inputs.OnPushCanceled?.RemoveListener(OnPushCanceled);
        }
            
    }

    protected virtual void OnInteract(Player player){}
    protected virtual void OnPush(Player player){}
    protected virtual void OnPushCanceled(Player player){}
    
    protected virtual void PlayAnimation(){}

}

public interface IResettable
{
    public abstract void ResetAsDefault();
}
    
