using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
    [SerializeField] private List<RoomRegion> _floor = new List<RoomRegion>();
    [SerializeField] private bool _reseting;
    private List<Room> _roomsInPlay = new List<Room>();
    SCRoomsLists _roomsLists;
    List<GameObject> _currentFloor = new List<GameObject>();

    #region Generation
    private void Start()
    {
        _roomsLists = Resources.Load<SCRoomsLists>("ScriptableObject/Rooms");
        _reseting = false;
        GenerateRooms();
    }
    private void GenerateRooms()
    {
        int i = 0;
        if (_roomsInPlay.Count > 0)
        {
            foreach (Room room in _roomsInPlay)
            {
                room.GetComponent<SelfDestruct>().Destruct();
            }
        }
        _roomsInPlay = new List<Room>();
        foreach (RoomRegion floor in _floor)
        {
            foreach (GameObject room in _roomsLists.Floors[i].Rooms)
            {
                _currentFloor.Add(room);
            }
            int rand = Random.Range(0, _currentFloor.Count);
            GameObject room1 = Instantiate(_currentFloor[rand], floor.FloorA.transform.position, floor.FloorA.transform.rotation);
            _currentFloor.RemoveAt(rand);
            Debug.Log(_currentFloor.Count);
            int rand2 = Random.Range(0, _currentFloor.Count);
            GameObject room2 = Instantiate(_currentFloor[rand2], floor.FloorB.transform.position, floor.FloorB.transform.rotation);
            _currentFloor.RemoveAt(rand2);
            _roomsInPlay.Add(room1.GetComponent<Room>());
            _roomsInPlay.Add(room2.GetComponent<Room>());
            i++;
            _currentFloor.Clear();
        }
        SetRooms();
    }
    #endregion
    #region Shuffle
    private void Update()
    {
        if (_reseting)
        {
            Shuffle();
            _reseting = false;
        }
    }
    private void Shuffle()
    {
        List<Vector3> roomsPos = new List<Vector3>();
        for(int i = 0; i < _floor.Count; i++)
        {
            roomsPos.Add(_floor[i].FloorA.transform.position);
            roomsPos.Add(_floor[i].FloorB.transform.position);
        }
        foreach(Room room in _roomsInPlay)
        {
            int rand = Random.Range(0, roomsPos.Count);
            room.transform.position = roomsPos[rand];
            roomsPos.RemoveAt(rand);
        }
        SetRooms();
    }
    #endregion
    public void SetRooms()
    {
        if (_floor.Count > 1)
        {
            foreach (Room room in _roomsInPlay)
            {
                for (int i = 0; i < _floor.Count; i++)
                {
                    if (i == 0)
                    {
                        if (room.transform.position == _floor[i].FloorA.transform.position)
                        {
                            room.PreviousRoom = null;
                            room.NextRoom = FindRoomAtPosition(_floor[i + 1].FloorA.transform.position);

                        }
                        if (room.transform.position == _floor[i].FloorB.transform.position)
                        {
                            room.PreviousRoom = null;
                            room.NextRoom = FindRoomAtPosition(_floor[i + 1].FloorB.transform.position);
                        }
                    }
                    else if (i == _floor.Count - 1)
                    {
                        if(room.transform.position == _floor[i].FloorA.transform.position)
                        {
                            room.PreviousRoom = FindRoomAtPosition(_floor[i - 1].FloorA.transform.position);
                            room.NextRoom = null;

                        }
                        if (room.transform.position == _floor[i].FloorB.transform.position)
                        {
                            room.PreviousRoom = FindRoomAtPosition(_floor[i - 1].FloorB.transform.position); ;
                            room.NextRoom = null;
                        }
                    }
                    else
                    {
                        if (room.transform.position == _floor[i].FloorA.transform.position)
                        {
                            room.PreviousRoom = FindRoomAtPosition(_floor[i - 1].FloorA.transform.position);
                            room.NextRoom = FindRoomAtPosition(_floor[i + 1].FloorA.transform.position);

                        }
                        if (room.transform.position == _floor[i].FloorB.transform.position)
                        {
                            room.PreviousRoom = FindRoomAtPosition(_floor[i - 1].FloorB.transform.position); ;
                            room.NextRoom = FindRoomAtPosition(_floor[i + 1].FloorB.transform.position);
                        }
                    }
                }
            }
        } 
    }
    public Room FindRoomAtPosition(Vector3 pos)
    {
        Room _wantedRoom = null;
        foreach (Room room in _roomsInPlay)
        {
            if (room.transform.position == pos)
            {
                _wantedRoom = room;
            }
        }
        return _wantedRoom;
    }
}
[System.Serializable]
public class RoomRegion
{
    [SerializeField] private GameObject _floorA;
    [SerializeField] private GameObject _floorB;

    public GameObject FloorA { get => _floorA; }
    public GameObject FloorB { get => _floorB; }
}
