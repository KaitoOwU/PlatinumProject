using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : Room
{
    public void SetCorridor(Player p,Door endDoor)
    {
        if (p.RelativePos == HubRelativePosition.LEFT_WING)
        {
            this.RoomSide = Side.LEFT;
        }
        else
        {
            this.RoomSide = Side.RIGHT;
        }
        this.Doors[1].LinkedDoor = endDoor;
        this.Doors[0].IsLocked = true;
    }
}
