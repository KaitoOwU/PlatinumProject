using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clue : Interactable
{
    [SerializeField] private ClueData _data;
    public ClueData Data
    {
        get => _data;
        set => _data = value;
    }

    protected override void OnInteract(Player player)
    {
        if (player.CurrentRoom.RoomSide == Room.Side.LEFT)
        {
            UIClue.left.Init(_data);
            Destroy(gameObject);
        } else if (player.CurrentRoom.RoomSide == Room.Side.RIGHT)
        {
            UIClue.right.Init(_data);
            Destroy(gameObject);
        }
    }
    
}
