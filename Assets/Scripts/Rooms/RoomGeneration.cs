using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoomGeneration : MonoBehaviour
{
    //[SerializeField] private List<RoomRegion> _floor = new List<RoomRegion>();
    [SerializeField] private bool _reseting;
    [SerializeField] private Hub _hall;
    private List<Room> _roomsInPlayR = new List<Room>();
    private List<Room> _roomsInPlayL = new List<Room>();
    private List<Room> _roomsInPlay = new List<Room>();
    SCRoomsLists _roomsLists;
    List<GameObject> _currentFloor = new List<GameObject>();
    [SerializeField] LayoutGenerator _layout;
    #region Generation
    private void Start()
    {
        _roomsLists = Resources.Load<SCRoomsLists>("ScriptableObject/Rooms");
        _reseting = false;
    }
    /*  private void GenerateRooms()
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
              int rand2 = Random.Range(0, _currentFloor.Count);
              GameObject room2 = Instantiate(_currentFloor[rand2], floor.FloorB.transform.position, floor.FloorB.transform.rotation);
              _currentFloor.RemoveAt(rand2);

              _roomsInPlay.Add(room1.GetComponent<Room>());
              _roomsInPlay.Add(room2.GetComponent<Room>());
              i++;
              _currentFloor.Clear();
          }
          SetRooms();
      }*/
    public void GenerateRooms()
    {
        _roomsInPlayR = new List<Room>();
        _roomsInPlayL = new List<Room>();
        int i = 0;
        Debug.Log(_layout.AisleLeft[0].Count);
        foreach(List<RoomPosition> positionsList in _layout.AisleLeft)
        {
            Debug.Log("i " + i);
            int rand = Random.Range(0, _currentFloor.Count);
            for (int j = 0; j < positionsList.Count; j++)
            {
                Debug.Log("j " + j);
                GameObject room= Instantiate(_roomsLists.Floors[i].Rooms[rand], positionsList[j].Position,transform.rotation);
                _roomsInPlayL.Add(room.GetComponent<Room>());
                _roomsInPlay.Add(room.GetComponent<Room>());
            }
            i++;
        }
        i = 0;
        foreach (List<RoomPosition> positionsList in _layout.AisleRight)
        {
            Debug.Log("i " + i);
            int rand = Random.Range(0, _roomsLists.Floors[i].Rooms.Count);
            for (int j = 0; j < positionsList.Count; j++)
            {
                Debug.Log("j " + j);
                GameObject room = Instantiate(_roomsLists.Floors[i].Rooms[rand], positionsList[j].Position, transform.rotation);
                _roomsInPlayR.Add(room.GetComponent<Room>());
                _roomsInPlay.Add(room.GetComponent<Room>());
            }
<<<<<<< Updated upstream
            int rand = Random.Range(0, _currentFloor.Count);
            GameObject room1 = Instantiate(_currentFloor[rand], floor.FloorA.transform.position, floor.FloorA.transform.rotation);
            _currentFloor.RemoveAt(rand);
            int rand2 = Random.Range(0, _currentFloor.Count);
            GameObject room2 = Instantiate(_currentFloor[rand2], floor.FloorB.transform.position, floor.FloorB.transform.rotation);
            _currentFloor.RemoveAt(rand2);

            _roomsInPlay.Add(room1.GetComponent<Room>());
            _roomsInPlay.Add(room2.GetComponent<Room>());
=======
>>>>>>> Stashed changes
            i++;
        }
    }
    #endregion
    #region Shuffle
    private void Update()
    {
        if (_reseting)
        {
            //Shuffle();
            _reseting = false;
        }
    }
   /* private void Shuffle()
    {
        List<Vector3> roomsPos = new List<Vector3>();
        for (int i = 0; i < _floor.Count - 1; i++)
        {
            roomsPos.Add(_floor[i].FloorA.transform.position);
            roomsPos.Add(_floor[i].FloorB.transform.position);
        }
        foreach (Room room in _roomsInPlay)
        {
            if (room != _roomsInPlay[_roomsInPlay.Count - 1] && room != _roomsInPlay[_roomsInPlay.Count - 2])
            {
                int rand = Random.Range(0, roomsPos.Count);
                room.transform.position = roomsPos[rand];
                roomsPos.RemoveAt(rand);
            }
        }
        SetRooms();
    }*/
    #endregion
   
    public void SetRooms()
    {

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
/*[System.Serializable]
public class RoomRegion
{
    [SerializeField] private GameObject _floorA;
    [SerializeField] private GameObject _floorB;

    public GameObject FloorA { get => _floorA; }
    public GameObject FloorB { get => _floorB; }
}*/
