using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rooms", menuName = "Platinum/Rooms", order = 0)]
public class SCRoomsLists : ScriptableObject
{

    [SerializeField] private List<Floor> _floors = new List<Floor>();
    public List<Floor>  Floors { get => _floors; set => _floors = value; }
}
[System.Serializable]
public class Floor
{
    [SerializeField] private List<GameObject> _rooms = new List<GameObject>();

    public List<GameObject> Rooms{ get => _rooms; set => _rooms = value; }
}
