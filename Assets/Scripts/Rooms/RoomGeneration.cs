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


    private void OnDisable()
    {
        GameManager.Instance.OnEndPhase.RemoveListener(Shuffle);
    }

    #region Generation
    private void Start()
    {
        GameManager.Instance.OnEndPhase.AddListener(Shuffle);
    }
    public void GenerateRooms()
    {
        _roomsInPlay.Add(_hall);
        _roomsLists = Resources.Load<SCRoomsLists>("ScriptableObject/Rooms");
        List<GameObject> roomsToPlace= new List<GameObject>();
        foreach(RoomPosition positionsList in _layout.AisleLeftInOrder)
        {
            foreach(GameObject room1 in _roomsLists.Floors[positionsList.DoorNumber-1].Rooms)
            {
                roomsToPlace.Add(room1);
            }            
            int rand = Random.Range(0, _roomsLists.Floors[positionsList.DoorNumber - 1].Rooms.Count);
            GameObject roomToBe = _roomsLists.Floors[positionsList.DoorNumber-1].Rooms[rand];
            if (roomToBe.GetComponent<Room>().Tandem != null && positionsList.DoorNumber == 2)
            {
                    _tandemToPlace.Add(roomToBe.GetComponent<Room>().Tandem);
                    
            }
            else if (roomToBe.GetComponent<Room>().Tandem != null)
            {
                while(roomToBe.GetComponent<Room>().Tandem != null)
                {
                     rand = Random.Range(0, _roomsLists.Floors[positionsList.DoorNumber - 1].Rooms.Count);
                     roomToBe = _roomsLists.Floors[positionsList.DoorNumber - 1].Rooms[rand];
                }
            }
            GameObject room= Instantiate(roomToBe, positionsList.Position,transform.rotation);
            room.GetComponent<Room>().RoomSide = Room.Side.LEFT;
            if (roomToBe.GetComponent<Room>().Tandem != null && positionsList.DoorNumber  == 2)
            {
                _tandemRoom.Add(room.GetComponent<Room>());
            }
            switch (positionsList.DoorNumber)
            {
                case 1:
                   _roomsInPlay1.Add(room.GetComponent<Room>());
                    break;
                case 2:
                    _roomsInPlay2.Add(room.GetComponent<Room>());
                    break;
                case 3:
                    _roomsInPlay3.Add(room.GetComponent<Room>());
                    break;
                case 4:
                    _roomsInPlay4.Add(room.GetComponent<Room>());
                    break;
             }               
            _roomsInPlay.Add(room.GetComponent<Room>());
        }
         roomsToPlace.Clear();      
        foreach (RoomPosition positionsList in _layout.AisleRightInOrder)
        {
            int count = 0;
            GameObject room;
            int rand = Random.Range(0, _roomsLists.Floors[positionsList.DoorNumber - 1].Rooms.Count);
            int rand2 = Random.Range(0, _layout.AisleRight[positionsList.DoorNumber - 1].Count -count);
            if (positionsList.DoorNumber == 2 && _tandemToPlace.Count > 0 && (rand2 == 0 || _layout.AisleRight[positionsList.DoorNumber - 1].Count - count == _tandemToPlace.Count))
            {
                room = Instantiate(_tandemToPlace[0].gameObject, positionsList.Position, transform.rotation);
                _tandemRoom.Add(room.GetComponent<Room>());
                _tandemToPlace.Remove(_tandemToPlace[0]);
                room.GetComponent<Room>().RoomSide = Room.Side.RIGHT;

            }
            else
            {
                if (_roomsLists.Floors[positionsList.DoorNumber - 1].Rooms[rand].GetComponent<Room>().Tandem != null)
                {
                    while (_roomsLists.Floors[positionsList.DoorNumber - 1].Rooms[rand].GetComponent<Room>().Tandem != null)
                    {
                        rand = Random.Range(0, _roomsLists.Floors[positionsList.DoorNumber - 1].Rooms.Count);
                    }
                }
                room = Instantiate(_roomsLists.Floors[positionsList.DoorNumber - 1].Rooms[rand], positionsList.Position, transform.rotation);
                room.GetComponent<Room>().RoomSide = Room.Side.RIGHT;
            }
            switch (positionsList.DoorNumber)
                {
                case 1:
                    _roomsInPlay1.Add(room.GetComponent<Room>());
                    break;
                case 2:
                    _roomsInPlay2.Add(room.GetComponent<Room>());
                    break;
                case 3:
                    _roomsInPlay3.Add(room.GetComponent<Room>());
                    break;
                case 4:
                    _roomsInPlay4.Add(room.GetComponent<Room>());
                    break;
            }
            _roomsInPlay.Add(room.GetComponent<Room>());
        } 
        for(int j = 0; j < _tandemRoom.Count / 2; j++)
        {
            _tandemRoom[j].Tandem = _tandemRoom[j + _tandemRoom.Count / 2];
            _tandemRoom[_tandemRoom.Count / 2 + j].Tandem = _tandemRoom[j];
        }
        SetRooms();
        LockedDoor();
    }
    #endregion
    #region Shuffle
    private void Shuffle()
    {
        List<Room> roomToShuffle1 = new List<Room>();
        List<Room> roomToShuffle2 = new List<Room>();
        List<Room> roomToShuffle3 = new List<Room>();
        List<Room> roomToShuffle4 = new List<Room>();
        List<Room> TandemToShuffle = new List<Room>();
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
        _roomsInPlay.Clear();
        _roomsInPlay1.Clear();
        _roomsInPlay2.Clear();
        _roomsInPlay3.Clear();
        _roomsInPlay4.Clear();
        foreach(RoomPosition roomPosition in _layout.AisleLeftInOrder)
        {
            int rand = 0;
            switch (roomPosition.DoorNumber-1)
                {
                    case 0:
                         rand = Random.Range(0, roomToShuffle1.Count);
                        roomToShuffle1[rand].transform.position = roomPosition.Position;
                         roomToShuffle1[rand].RoomSide = Room.Side.LEFT;
                        _roomsInPlay1.Add(roomToShuffle1[rand]);
                        _roomsInPlay.Add(roomToShuffle1[rand]);
                        roomToShuffle1.Remove(roomToShuffle1[rand]);
                        break;
                    case 1:
                        int rand2 = Random.Range(0, roomToShuffle2.Count - _layout.AisleRight[1].Count+(TandemToShuffle.Count - _tandemRoom.Count / 2)) ;
                        if (TandemToShuffle.Count > _tandemRoom.Count / 2 && (rand2 == 1 || roomToShuffle2.Count-_layout.AisleRight[1].Count == 0))
                        {
                            TandemToShuffle[TandemToShuffle.Count - 1].transform.position = roomPosition.Position;
                            TandemToShuffle[TandemToShuffle.Count - 1].RoomSide = Room.Side.LEFT;
                            _roomsInPlay2.Add(TandemToShuffle[TandemToShuffle.Count - 1]);
                            _roomsInPlay.Add(TandemToShuffle[TandemToShuffle.Count - 1]);
                            TandemToShuffle.Remove(TandemToShuffle[TandemToShuffle.Count - 1]);
                        }
                        else
                        {
                            rand = Random.Range(0, roomToShuffle2.Count);   
                            roomToShuffle2[rand].transform.position = roomPosition.Position;
                            roomToShuffle2[rand].RoomSide = Room.Side.LEFT;
                            _roomsInPlay2.Add(roomToShuffle2[rand]);
                            _roomsInPlay.Add(roomToShuffle2[rand]);
                            roomToShuffle2.Remove(roomToShuffle2[rand]);
                        }
                        break;
                    case 2:
                        rand = Random.Range(0, roomToShuffle3.Count);
                        roomToShuffle3[rand].transform.position = roomPosition.Position;
                        roomToShuffle3[rand].RoomSide = Room.Side.LEFT;
                        _roomsInPlay3.Add(roomToShuffle3[rand]);
                        _roomsInPlay.Add(roomToShuffle3[rand]);
                        roomToShuffle3.Remove(roomToShuffle3[rand]);
                        break;
                    case 3:
                        rand = Random.Range(0, roomToShuffle4.Count);
                        roomToShuffle4[rand].transform.position = roomPosition.Position;
                        roomToShuffle4[rand].RoomSide = Room.Side.LEFT;
                        _roomsInPlay4.Add(roomToShuffle4[rand]);
                        _roomsInPlay.Add(roomToShuffle4[rand]);
                        roomToShuffle4.Remove(roomToShuffle4[rand]);
                        break;
                }
        }
            foreach (RoomPosition roomPosition in _layout.AisleRightInOrder)
            {
                int rand = 0;
                switch (roomPosition.DoorNumber-1)
                {
                    case 0:
                        rand = Random.Range(0, roomToShuffle1.Count);
                        roomToShuffle1[rand].transform.position = roomPosition.Position;
                        roomToShuffle1[rand].RoomSide = Room.Side.RIGHT;
                        _roomsInPlay1.Add(roomToShuffle1[rand]);
                        _roomsInPlay.Add(roomToShuffle1[rand]);
                        roomToShuffle1.Remove(roomToShuffle1[rand]);
                        break;
                    case 1:
                        int rand2 = Random.Range(0, TandemToShuffle.Count + roomToShuffle2.Count - 1);
                        if (TandemToShuffle.Count > 0 && (rand2 == 0 || roomToShuffle2.Count == 0))
                        {
                            TandemToShuffle[TandemToShuffle.Count-1].transform.position = roomPosition.Position;
                            TandemToShuffle[TandemToShuffle.Count - 1].RoomSide = Room.Side.RIGHT;
                            _roomsInPlay2.Add(TandemToShuffle[TandemToShuffle.Count - 1]);
                            _roomsInPlay.Add(TandemToShuffle[TandemToShuffle.Count - 1]);
                            TandemToShuffle.Remove(TandemToShuffle[TandemToShuffle.Count - 1]);                      
                        }
                        else
                        {
                            rand = Random.Range(0, roomToShuffle2.Count);
                            roomToShuffle2[rand].transform.position = roomPosition.Position;
                            roomToShuffle2[rand].RoomSide = Room.Side.RIGHT;
                            _roomsInPlay2.Add(roomToShuffle2[rand]);
                            _roomsInPlay.Add(roomToShuffle2[rand]);
                            roomToShuffle2.Remove(roomToShuffle2[rand]);
                        }
                        break;
                    case 2:
                        rand = Random.Range(0, roomToShuffle3.Count);
                        roomToShuffle3[rand].transform.position = roomPosition.Position;
                        roomToShuffle3[rand].RoomSide = Room.Side.RIGHT;
                        _roomsInPlay3.Add(roomToShuffle3[rand]);
                        _roomsInPlay.Add(roomToShuffle3[rand]);
                        roomToShuffle3.Remove(roomToShuffle3[rand]);
                        break;
                    case 3:
                        rand = Random.Range(0, roomToShuffle4.Count);
                        roomToShuffle4[rand].transform.position = roomPosition.Position;
                        roomToShuffle4[rand].RoomSide = Room.Side.RIGHT;
                        _roomsInPlay4.Add(roomToShuffle4[rand]);
                        _roomsInPlay.Add(roomToShuffle4[rand]);
                        roomToShuffle4.Remove(roomToShuffle4[rand]);
                        break;
                }
            }
        SetRooms();
        LockedDoor();
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
            room.name = "room "+room.Doors.Count+ " doors : " + room.transform.position;
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
    private void LockedDoor()
    {
        foreach(Room room in _roomsInPlay)
        {
            foreach(Door door in room.Doors)
            {
                door.IsLocked = true;
            }
        }
        int max=0;
        switch (FindObjectOfType<GameManager>().CurrentTimerPhase)
        {
            case GameManager.TimerPhase.FIRST_PHASE:
                max = 2;
                break;
            case GameManager.TimerPhase.SECOND_PHASE:
                max =4;
                break;
            case GameManager.TimerPhase.THIRD_PHASE:
                max = _roomsInPlay.Count / 2-1;
                break;
        }
        for (int i = 0; i < max; i++)
        {
            foreach (Door door in FindRoomAtPosition(_layout.AisleLeftInOrder[i].Position).Doors)
            {
                door.IsLocked = false;
            }
            foreach(Door door in FindRoomAtPosition(_layout.AisleRightInOrder[i].Position).Doors)
            {
                door.IsLocked = false;
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
