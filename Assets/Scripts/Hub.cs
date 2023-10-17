using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hub : Room
{
    [SerializeField] private Room _roomLeft;
    [SerializeField] private Room _roomRight;

    [SerializeField] private Door _roomDoorLeft;
    [SerializeField] private Door _roomDoorRight;

    public Room RoomLeft { get => _roomLeft; set => _roomLeft = value; }
    public Room RoomRight { get => _roomRight; set => _roomRight = value; }
    public Door RoomDoorLeft { get => _roomDoorLeft; set => _roomDoorLeft = value; }
    public Door RoomDoorRight { get => _roomDoorRight; set => _roomDoorRight = value; }

}
