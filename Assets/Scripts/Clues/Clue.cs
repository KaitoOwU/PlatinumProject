using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

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

        GameManager.Instance.FoundClues.Add(_data);

        if (player.CurrentRoom.RoomSide == Room.Side.LEFT)
        {
            UIItemFrameManager.instance.ClueAcquired(_data);
            UIClue.left.Init(_data);
            OnGetClues?.Invoke();
            Destroy(gameObject);
        } else if (player.CurrentRoom.RoomSide == Room.Side.RIGHT)
        {
            UIItemFrameManager.instance.ClueAcquired(_data);
            UIClue.right.Init(_data);
            OnGetClues?.Invoke();
            Destroy(gameObject);
        }
    }
    
}
