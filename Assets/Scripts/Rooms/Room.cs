using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    
    [SerializeField] private Room _nextRoom;
    [SerializeField] private Room _previousRoom;
    [SerializeField] private Vector3 position;

    public Room NextRoom { get => _nextRoom; set => _nextRoom = value; }
    public Room PreviousRoom { get => _previousRoom; set => _previousRoom = value; }
    public Vector3 Position { get => position; set => position = value; }
}
