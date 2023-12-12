using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    protected List<Player> _playersInRange = new();
    protected Collider _collider;
    public IReadOnlyList<Player> PlayersInRange => _playersInRange;

    [Header("--- Messages ---")]
    [SerializeField]
    protected string _onRangeMessage;
    [SerializeField]
    protected string _onInteractMessage;

    protected UnityEvent _onPlayerEnterRange;
    protected UnityEvent _onPlayerExitRange;

    protected Bubble _message;


    protected virtual void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;

        PlayerController p = other.GetComponent<PlayerController>();

        if (p.Inputs == null)
            return;
        
        _playersInRange.Add(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);

        p.Inputs.OnInteract.AddListener(OnInteract);
        
        _onPlayerEnterRange?.Invoke();
        
        if ((_message == null || !_message.gameObject.activeSelf) && _onRangeMessage != "")
        {
            _message = TutorialManager.Instance.ShowBubbleMessage(p.PlayerIndex, transform, p.Inputs.ControllerIndex, _onRangeMessage);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;
        
        PlayerController p = other.GetComponent<PlayerController>();
        
        if (p.Inputs == null)
            return;
        
        _playersInRange.Remove(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);
        
        p.Inputs.OnInteract?.RemoveListener(OnInteract);

        _onPlayerExitRange?.Invoke();
    }

    protected virtual void OnDestroy()
    {
        foreach (Player p in _playersInRange)
        {
            GameManager.Instance.PlayerList[p.Index - 1].PlayerController.Inputs.OnInteract?.RemoveListener(OnInteract);
        }
            
    }

    protected virtual void OnInteract(Player player)
    {
        if ((_message == null || !_message.gameObject.activeSelf) && _onInteractMessage != "")
        {
            _message = TutorialManager.Instance.ShowBubbleMessage(player.Index, transform, player.PlayerController.Inputs.ControllerIndex, _onInteractMessage);
        }
    }
    protected virtual void PlayAnimation(){}

}

public interface IResettable
{
    public abstract void ResetAsDefault();
}
    
