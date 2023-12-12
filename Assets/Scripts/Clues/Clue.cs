using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Clue : Interactable
{
    [HideInInspector] public UnityEvent OnGetClues;
    [SerializeField] private ClueData _data;
    public ClueData Data
    {
        get => _data;
        set => _data = value;
    }

    protected override void OnInteract(Player player)
    {
        base.OnInteract(player);

        
        if (player.CurrentRoom.RoomSide == Room.Side.LEFT || player.CurrentRoom.RoomSide == Room.Side.HUB)
        {
            UIItemFrameManager.instance.ClueAcquired(_data);
            OnGetClues?.Invoke();
            Destroy(gameObject);
        } else if (player.CurrentRoom.RoomSide == Room.Side.RIGHT)
        {
            UIItemFrameManager.instance.ClueAcquired(_data);
            OnGetClues?.Invoke();
            Destroy(gameObject);
        }
    }
    
}
