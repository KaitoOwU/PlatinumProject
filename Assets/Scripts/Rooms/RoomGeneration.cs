using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoomGeneration : MonoBehaviour
{
    //[SerializeField] private List<RoomRegion> _floor = new List<RoomRegion>();
    [SerializeField] private bool _reseting;
    [SerializeField] private Hub _hall;
    private List<Room> _roomsInPlay1 = new List<Room>();
    private List<Room> _roomsInPlay2 = new List<Room>();
    private List<Room> _roomsInPlay3 = new List<Room>();
    private List<Room> _roomsInPlay4 = new List<Room>();
    private List<Room> _roomsInPlay = new List<Room>();
    private List<Room> _tandemToPlace = new List<Room>();
    private List<Room> _tandemRoom = new List<Room>();
    SCRoomsLists _roomsLists;
    [SerializeField] LayoutGenerator _layout;
    #region Generation
    private void Start()
    {
       
        _reseting = false;
    }
    #region Old
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
    #endregion;
    public void GenerateRooms()
    {
        _roomsInPlay.Add(_hall);
        _roomsLists = Resources.Load<SCRoomsLists>("ScriptableObject/Rooms");
        List<GameObject> roomsToPlace= new List<GameObject>();
            int i = 0;
        foreach(List<RoomPosition> positionsList in _layout.AisleLeft)
        {
            foreach(GameObject room1 in _roomsLists.Floors[i].Rooms)
            {
                roomsToPlace.Add(room1);
            }

            //Debug.Log("i " + i);
            for (int j = 0; j < positionsList.Count; j++)
            {              
                 int rand = Random.Range(0, _roomsLists.Floors[i].Rooms.Count);
                GameObject roomToBe = _roomsLists.Floors[i].Rooms[rand];
                if (roomToBe.GetComponent<Room>().Tandem != null && i == 1&&j<_layout.AisleRight[1].Count)
                {
                    _tandemToPlace.Add(roomToBe.GetComponent<Room>().Tandem);
                    
                }
                else if (roomToBe.GetComponent<Room>().Tandem != null)
                {
                    --j;
                    continue;
                }
                //Debug.Log("j " + j);
                GameObject room= Instantiate(roomToBe, positionsList[j].Position,transform.rotation);
                 room.GetComponent<Room>().RoomSide = Room.Side.LEFT;
                if (roomToBe.GetComponent<Room>().Tandem != null && i == 1)
                {
                    _tandemRoom.Add(room.GetComponent<Room>());
                }

                switch (i)
                {
                    case 0:
                        _roomsInPlay1.Add(room.GetComponent<Room>());
                        break;
                    case 1:
                        _roomsInPlay2.Add(room.GetComponent<Room>());
                        break;
                    case 2:
                        _roomsInPlay3.Add(room.GetComponent<Room>());
                        break;
                    case 3:
                        _roomsInPlay4.Add(room.GetComponent<Room>());
                        break;
                 }               
                _roomsInPlay.Add(room.GetComponent<Room>());
            }
            i++;
            roomsToPlace.Clear();
            if (i > 4)
                break;
        }
        i = 0;
        foreach (List<RoomPosition> positionsList in _layout.AisleRight)
        {
            //Debug.Log("i " + i);
            for (int j = 0; j < positionsList.Count; j++)
            {
                //Debug.Log("j " + j);
                GameObject room;
                int rand = Random.Range(0, _roomsLists.Floors[i].Rooms.Count);
                if (i == 1 && _tandemToPlace.Count > 0)
                {
                    room = Instantiate(_tandemToPlace[0].gameObject, positionsList[j].Position, transform.rotation);
                    _tandemRoom.Add(room.GetComponent<Room>());
                    _tandemToPlace.Remove(_tandemToPlace[0]);
                    room.GetComponent<Room>().RoomSide = Room.Side.RIGHT;

                }
                else if (_roomsLists.Floors[i].Rooms[rand].GetComponent<Room>().Tandem!=null)
                {
                    --j;
                    continue;
                }
                else
                {
                    room = Instantiate(_roomsLists.Floors[i].Rooms[rand], positionsList[j].Position, transform.rotation);
                    room.GetComponent<Room>().RoomSide = Room.Side.RIGHT;

                }
                switch (i)
                {
                    case 0:
                        _roomsInPlay1.Add(room.GetComponent<Room>());
                        break;
                    case 1:
                        _roomsInPlay2.Add(room.GetComponent<Room>());
                        break;
                    case 2:
                        _roomsInPlay3.Add(room.GetComponent<Room>());
                        break;
                    case 3:
                        _roomsInPlay4.Add(room.GetComponent<Room>());
                        break;
                }
                _roomsInPlay.Add(room.GetComponent<Room>());
            }
            i++;
            if(i > 4)
            {
                break;
            }
 
        }
        for(int j = 0; j < _tandemRoom.Count / 2; j++)
        {
            Debug.Log(j +"Sa m�re");
            _tandemRoom[j].Tandem = _tandemRoom[j + _tandemRoom.Count / 2];
            _tandemRoom[_tandemRoom.Count / 2 + j].Tandem = _tandemRoom[j];
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
    #region Old
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
    private void Shuffle()
    {
        List<Room> roomToShuffle1 = new List<Room>();
        List<Room> roomToShuffle2 = new List<Room>();
        List<Room> roomToShuffle3 = new List<Room>();
        List<Room> roomToShuffle4 = new List<Room>();
        List<Room> TandemToShuffle = new List<Room>();
        foreach(Room room in _tandemRoom)
        {
            
        }
        foreach(Room room in _roomsInPlay1)
        {
            roomToShuffle1.Add(room);
        }
        foreach (Room room in _roomsInPlay2)
        {
            if (room.Tandem != null)
            {
                TandemToShuffle.Add(room);
            }
            else
            {
                roomToShuffle2.Add(room);
            }
        }
        foreach (Room room in _roomsInPlay3)
        {
            roomToShuffle3.Add(room);
        }
        foreach (Room room in _roomsInPlay4)
        {
            roomToShuffle4.Add(room);
        }
        _tandemToPlace.Clear();
        _roomsInPlay1.Clear();
        _roomsInPlay2.Clear();
        _roomsInPlay3.Clear();
        _roomsInPlay4.Clear();
        int i = 0;
        foreach (List<RoomPosition> roomPositions in _layout.AisleLeft)
        {
            foreach(RoomPosition roomPosition in roomPositions)
            {
                int rand = 0;
                switch (i)
                {
                    case 0:
                         rand = Random.Range(0, roomToShuffle1.Count);
                        roomToShuffle1[rand].transform.position = roomPosition.Position;
                         roomToShuffle1[rand].RoomSide = Room.Side.LEFT;
                        _roomsInPlay1.Add(roomToShuffle1[rand]);
                        roomToShuffle1.Remove(roomToShuffle1[rand]);
                        break;
                    case 1:
                        int rand2 = Random.Range(0, roomToShuffle2.Count - _layout.AisleRight[1].Count+(TandemToShuffle.Count - _tandemRoom.Count / 2)) ;
                        Debug.Log(rand2);
                        if (TandemToShuffle.Count > _tandemRoom.Count / 2 && (rand2 == 1 || roomToShuffle2.Count-_layout.AisleRight[1].Count == 0))
                        {
                            TandemToShuffle[TandemToShuffle.Count - 1].transform.position = roomPosition.Position;
                            TandemToShuffle[TandemToShuffle.Count - 1].RoomSide = Room.Side.LEFT;
                            _roomsInPlay2.Add(TandemToShuffle[TandemToShuffle.Count - 1]);
                            TandemToShuffle.Remove(TandemToShuffle[TandemToShuffle.Count - 1]);
                        }
                        else
                        {
                            rand = Random.Range(0, roomToShuffle2.Count);
                            roomToShuffle2[rand].transform.position = roomPosition.Position;
                            roomToShuffle2[rand].RoomSide = Room.Side.LEFT;
                            _roomsInPlay2.Add(roomToShuffle2[rand]);
                            roomToShuffle2.Remove(roomToShuffle2[rand]);
                        }
                        break;
                    case 2:
                        rand = Random.Range(0, roomToShuffle3.Count);
                        roomToShuffle3[rand].transform.position = roomPosition.Position;
                        roomToShuffle3[rand].RoomSide = Room.Side.LEFT;
                        _roomsInPlay3.Add(roomToShuffle3[rand]);
                        roomToShuffle3.Remove(roomToShuffle3[rand]);
                        break;
                    case 3:
                        rand = Random.Range(0, roomToShuffle4.Count);
                        roomToShuffle4[rand].transform.position = roomPosition.Position;
                        roomToShuffle4[rand].RoomSide = Room.Side.LEFT;
                        _roomsInPlay4.Add(roomToShuffle4[rand]);
                        roomToShuffle4.Remove(roomToShuffle4[rand]);
                        break;
                }
            }
            i++;
            if (i > 4)
                break;
        }
        i = 0;
        foreach (List<RoomPosition> roomPositions in _layout.AisleRight)
        {
            foreach (RoomPosition roomPosition in roomPositions)
            {
                int rand = 0;
                switch (i)
                {
                    case 0:
                        rand = Random.Range(0, roomToShuffle1.Count);
                        roomToShuffle1[rand].transform.position = roomPosition.Position;
                        roomToShuffle1[rand].RoomSide = Room.Side.RIGHT;
                        _roomsInPlay1.Add(roomToShuffle1[rand]);
                        roomToShuffle1.Remove(roomToShuffle1[rand]);
                        break;
                    case 1:
                        int rand2 = Random.Range(0, TandemToShuffle.Count+roomToShuffle2.Count-1);
                        Debug.Log(rand2);
                        if (TandemToShuffle.Count > 0 && (rand2 == 0 || roomToShuffle2.Count == 0))
                        {
                            TandemToShuffle[TandemToShuffle.Count-1].transform.position = roomPosition.Position;
                            TandemToShuffle[TandemToShuffle.Count - 1].RoomSide = Room.Side.RIGHT;
                            _roomsInPlay2.Add(TandemToShuffle[TandemToShuffle.Count - 1]);
                            TandemToShuffle.Remove(TandemToShuffle[TandemToShuffle.Count - 1]);                      
                        }
                        else
                        {
                            rand = Random.Range(0, roomToShuffle2.Count);
                            roomToShuffle2[rand].transform.position = roomPosition.Position;
                            roomToShuffle2[rand].RoomSide = Room.Side.RIGHT;
                            _roomsInPlay2.Add(roomToShuffle2[rand]);
                            roomToShuffle2.Remove(roomToShuffle2[rand]);
                        }
                        break;
                    case 2:
                        rand = Random.Range(0, roomToShuffle3.Count);
                        roomToShuffle3[rand].transform.position = roomPosition.Position;
                        roomToShuffle3[rand].RoomSide = Room.Side.RIGHT;
                        _roomsInPlay3.Add(roomToShuffle3[rand]);
                        roomToShuffle3.Remove(roomToShuffle3[rand]);
                        break;
                    case 3:
                        rand = Random.Range(0, roomToShuffle4.Count);
                        roomToShuffle4[rand].transform.position = roomPosition.Position;
                        roomToShuffle4[rand].RoomSide = Room.Side.RIGHT;
                        _roomsInPlay4.Add(roomToShuffle4[rand]);
                        roomToShuffle4.Remove(roomToShuffle4[rand]);
                        break;
                }
            }
            i++;
            if (i > 4)
                break;
        }
        SetRooms();
    }
    
    #endregion

    public void SetRooms()
    {
        Door[] allDoors = FindObjectsOfType<Door>();
        for (int i = 0; i < allDoors.Length; i++)
        {
            if (allDoors[i].LinkedDoor != null)
                allDoors[i].LinkedDoor = null;
        }
        foreach (Room room in _roomsInPlay)
        {
            room.LinkedRooms.Clear();
        }
        LinkRoom(_hall, FindRoomAtPosition(_hall.transform.position - new Vector3(_layout.BetweenRoomDistance, 0, 0)),_hall.Doors[0]);
        LinkRoom(_hall, FindRoomAtPosition(_hall.transform.position + new Vector3(_layout.BetweenRoomDistance, 0, 0)),_hall.Doors[1]);
        foreach (Room room in _roomsInPlay)
        {
            foreach(Door door in room.Doors)
            {
                if (door.LinkedDoor == null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                if (FindRoomAtPosition(room.transform.position + new Vector3(_layout.BetweenRoomDistance, 0, 0)))
                                {
                                    if(!FindRoomAtPosition(room.transform.position + new Vector3(_layout.BetweenRoomDistance, 0, 0)).LinkedRooms.Contains(room))
                                    {
                                        LinkRoom(room, FindRoomAtPosition(room.transform.position + new Vector3(_layout.BetweenRoomDistance, 0, 0)), door);
                                        i = 4;
                                    }
                                }
                                break;
                            case 1:
                                if (FindRoomAtPosition(room.transform.position + new Vector3(0, 0, _layout.BetweenRoomDistance)))
                                {
                                    if (!FindRoomAtPosition(room.transform.position + new Vector3(0, 0, _layout.BetweenRoomDistance)).LinkedRooms.Contains(room))
                                    {
                                        LinkRoom(room, FindRoomAtPosition(room.transform.position + new Vector3(0, 0, _layout.BetweenRoomDistance)), door);
                                        i = 4;
                                    }
                                }
                                break;
                            case 2:
                                if (FindRoomAtPosition(room.transform.position - new Vector3(_layout.BetweenRoomDistance, 0, 0)))
                                {
                                    if (!FindRoomAtPosition(room.transform.position - new Vector3(_layout.BetweenRoomDistance, 0, 0)).LinkedRooms.Contains(room))
                                    {
                                        LinkRoom(room, FindRoomAtPosition(room.transform.position - new Vector3(_layout.BetweenRoomDistance, 0, 0)), door);
                                        i = 4;
                                    }
                                }
                                break;
                            case 3:
                                if (FindRoomAtPosition(room.transform.position - new Vector3(0, 0, _layout.BetweenRoomDistance)))
                                {
                                    if (!FindRoomAtPosition(room.transform.position - new Vector3(0, 0, _layout.BetweenRoomDistance)).LinkedRooms.Contains(room))
                                    {
                                        LinkRoom(room, FindRoomAtPosition(room.transform.position - new Vector3(0, 0, _layout.BetweenRoomDistance)), door);
                                        i = 4;
                                    }
                                }
                                break;
                        }
                        
                    }
                }
            }
        }
    }
    private void LinkRoom(Room room, Room roomToLink,Door door)
    {
        foreach (Door doorToLink in roomToLink.Doors)
        {
            if (doorToLink.LinkedDoor == null)
            {
                room.LinkedRooms.Add(roomToLink);
               roomToLink.LinkedRooms.Add(room);
                door.LinkedDoor = doorToLink;
                doorToLink.LinkedDoor = door;
                
                break;
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
#region Old
/*[System.Serializable]
public class RoomRegion
{
    [SerializeField] private GameObject _floorA;
    [SerializeField] private GameObject _floorB;

    public GameObject FloorA { get => _floorA; }
    public GameObject FloorB { get => _floorB; }
}*/
#endregion
