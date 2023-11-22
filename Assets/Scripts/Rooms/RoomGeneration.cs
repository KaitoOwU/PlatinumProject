using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomGeneration : MonoBehaviour
{
    //[SerializeField] private List<RoomRegion> _floor = new List<RoomRegion>();
    [SerializeField] private Transform _generationParent;
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
    int _maxRooms;
    private List<GameObject> _rewards= new List<GameObject>();
    [SerializeField] private GameObject arm;
    private List<GameObject> _rewardClueOnly = new List<GameObject>();
    [SerializeField] private List<Material> _doormattMaterials = new List<Material>();

    public LayoutGenerator Layout { get => _layout; }

    private void Awake()
    {
        _roomsLists = Resources.Load<SCRoomsLists>("ScriptableObject/Rooms"); 
    }

    private void OnDisable()
    {
        GameManager.Instance.OnEachEndPhase.RemoveListener(Shuffle);
    }

    #region Generation
    private void Start()
    {
        _maxRooms = 0;
        GameManager.Instance.OnEachEndPhase.AddListener(Shuffle);
  
    }
    public IEnumerator GenerateRooms()
    {
        yield return null;
        Vector3 pos = new Vector3(0, 0, 55);
        _roomsInPlay.Add(_hall);
        for (int i = 1; i <= _roomsLists.Floors[4].Rooms.Count; i++)
        {
            GameObject Corridor = Instantiate(_roomsLists.Floors[4].Rooms[i - 1], pos * i, transform.rotation, _generationParent);
            Corridor.name = "Corridor " + i;
        }
        List<GameObject> roomsToPlace= new List<GameObject>();
        foreach(RoomPosition positionsList in _layout.AisleLeftInOrder)
        {
            foreach(GameObject room1 in _roomsLists.Floors[positionsList.DoorNumber-1].Rooms)
            {
                roomsToPlace.Add(room1);
            }            
            int rand = Random.Range(0, _roomsLists.Floors[positionsList.DoorNumber - 1].Rooms.Count);
            GameObject roomToBe = _roomsLists.Floors[positionsList.DoorNumber-1].Rooms[rand];
            Room roomTo = roomToBe.GetComponent<Room>();
            if (roomTo.Tandem != null && positionsList.DoorNumber == 2)
            {
                    _tandemToPlace.Add(roomTo.Tandem);
                    
            }
            else if (roomTo.Tandem != null)
            {
                while(roomTo.Tandem != null)
                {
                     rand = Random.Range(0, _roomsLists.Floors[positionsList.DoorNumber - 1].Rooms.Count);
                     roomToBe = _roomsLists.Floors[positionsList.DoorNumber - 1].Rooms[rand];
                }
            }
            GameObject roomObj= Instantiate(roomToBe, positionsList.Position,transform.rotation, _generationParent);
            Room room = roomObj.GetComponent<Room>(); 
            room.RoomSide = Room.Side.LEFT;
            if (roomTo.Tandem != null && positionsList.DoorNumber  == 2)
            {
                _tandemRoom.Add(room);
            }
            switch (positionsList.DoorNumber)
            {
                case 1:
                   _roomsInPlay1.Add(room);
                    break;
                case 2:
                    _roomsInPlay2.Add(room);
                    break;
                case 3:
                    _roomsInPlay3.Add(room);
                    break;
                case 4:
                    _roomsInPlay4.Add(room);
                    break;
             }               
            _roomsInPlay.Add(room);
        }
         roomsToPlace.Clear();      
        foreach (RoomPosition positionsList in _layout.AisleRightInOrder)
        {
            int count = 0;
            GameObject roomObj;
            Room room;
            int rand = Random.Range(0, _roomsLists.Floors[positionsList.DoorNumber - 1].Rooms.Count);
            int rand2 = Random.Range(0, _layout.AisleRight[positionsList.DoorNumber - 1].Count -count);
           
            if (positionsList.DoorNumber == 2 && _tandemToPlace.Count > 0 && (rand2 == 0 || _layout.AisleRight[positionsList.DoorNumber - 1].Count - count == _tandemToPlace.Count))
            {
                roomObj = Instantiate(_tandemToPlace[0].gameObject, positionsList.Position, transform.rotation, _generationParent);
                 room = roomObj.GetComponent<Room>();
                _tandemRoom.Add(room);
                _tandemToPlace.Remove(_tandemToPlace[0]);
                room.RoomSide = Room.Side.RIGHT;

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
                roomObj = Instantiate(_roomsLists.Floors[positionsList.DoorNumber - 1].Rooms[rand], positionsList.Position, transform.rotation, _generationParent);
                 room = roomObj.GetComponent<Room>();
                room.RoomSide = Room.Side.RIGHT;
            }
            switch (positionsList.DoorNumber)
                {
                case 1:
                    _roomsInPlay1.Add(room);
                    break;
                case 2:
                    _roomsInPlay2.Add(room);
                    break;
                case 3:
                    _roomsInPlay3.Add(room);
                    break;
                case 4:
                    _roomsInPlay4.Add(room);
                    break;
            }
            _roomsInPlay.Add(room);
        } 
        for(int j = 0; j < _tandemRoom.Count / 2; j++)
        {
            _tandemRoom[j].Tandem = _tandemRoom[j + _tandemRoom.Count / 2];
            _tandemRoom[_tandemRoom.Count / 2 + j].Tandem = _tandemRoom[j];
        }
       
        SetRooms();
        //LockedDoor();
        GameManager.Instance.DistributeClues();
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
        List<Room> TandemTotal= new List<Room>();
        for (int j = 1; j <= _maxRooms; j++)
        {
            switch (_roomsInPlay[j].Doors.Count)
            {
                case 1:
                    roomToShuffle1.Add(_roomsInPlay[j]);
                    break;
                case 2:
                    if (_roomsInPlay[j].Tandem != null)
                    {
                        TandemToShuffle.Add(_roomsInPlay[j]);
                        TandemTotal.Add(_roomsInPlay[j]);
                    }
                    else
                        roomToShuffle2.Add(_roomsInPlay[j]);
                    break;
                case 3:
                    roomToShuffle3.Add(_roomsInPlay[j]);
                    break;
                case 4:
                    roomToShuffle4.Add(_roomsInPlay[j]);
                    break;
            }
        }
        for (int j = _roomsInPlay.Count/2+1; j < _roomsInPlay.Count / 2+1+ _maxRooms; j++)
        {
            switch (_roomsInPlay[j].Doors.Count)
            {
                case 1:
                    roomToShuffle1.Add(_roomsInPlay[j]);
                    break;
                case 2:
                    if (_roomsInPlay[j].Tandem != null)
                    {
                        TandemToShuffle.Add(_roomsInPlay[j]);
                        TandemTotal.Add(_roomsInPlay[j]);
                    }
                    else
                        roomToShuffle2.Add(_roomsInPlay[j]);
                    break;
                case 3:
                    roomToShuffle3.Add(_roomsInPlay[j]);
                    break;
                case 4:
                    roomToShuffle4.Add(_roomsInPlay[j]);
                    break;
            }
        }
        int i = 0;
        foreach (RoomPosition roomPosition in _layout.AisleLeftInOrder)
        {        
            int rand = 0;
            switch (roomPosition.DoorNumber-1)
            {
                case 0:
                    rand = Random.Range(0, roomToShuffle1.Count);
                    roomToShuffle1[rand].transform.position = roomPosition.Position;
                    roomToShuffle1[rand].RoomSide = Room.Side.LEFT;
                    roomToShuffle1.Remove(roomToShuffle1[rand]);
                    break;
                case 1:
                    int rand2 = Random.Range(0, roomToShuffle2.Count - _layout.AisleRight[1].Count + (TandemToShuffle.Count - _tandemRoom.Count / 2));
                    if (TandemToShuffle.Count > TandemTotal.Count / 2 && (rand2 == 1 || roomToShuffle2.Count - _layout.AisleRight[1].Count == 0))
                    {
                        rand = Random.Range(0, TandemToShuffle.Count);
                        TandemToShuffle[rand].transform.position = roomPosition.Position;
                        TandemToShuffle[rand].RoomSide = Room.Side.LEFT;
                        TandemToShuffle.Remove(TandemToShuffle[rand]);
                    }
                    else
                    {
                        rand = Random.Range(0, roomToShuffle2.Count);
                        roomToShuffle2[rand].transform.position = roomPosition.Position;
                        roomToShuffle2[rand].RoomSide = Room.Side.LEFT;
                        roomToShuffle2.Remove(roomToShuffle2[rand]);
                    }
                    break;
                case 2:
                    rand = Random.Range(0, roomToShuffle3.Count);
                    roomToShuffle3[rand].transform.position = roomPosition.Position;
                    roomToShuffle3[rand].RoomSide = Room.Side.LEFT;
                    roomToShuffle3.Remove(roomToShuffle3[rand]);
                    break;
                case 3:
                    rand = Random.Range(0, roomToShuffle4.Count);
                    roomToShuffle4[rand].transform.position = roomPosition.Position;
                    roomToShuffle4[rand].RoomSide = Room.Side.LEFT;
                    roomToShuffle4.Remove(roomToShuffle4[rand]);
                    break;
            }
            i++;
            if (i >= _maxRooms)
            {
                break;
            }
        }
        i = _roomsInPlay.Count / 2 + 1;
        foreach (RoomPosition roomPosition in _layout.AisleRightInOrder)
        {
            int rand = 0;
            switch (roomPosition.DoorNumber-1)
            {
                case 0:
                    rand = Random.Range(0, roomToShuffle1.Count);
                    roomToShuffle1[rand].transform.position = roomPosition.Position;
                    roomToShuffle1[rand].RoomSide = Room.Side.RIGHT;
                    roomToShuffle1.Remove(roomToShuffle1[rand]);
                    break;
                case 1:
                    int rand2 = Random.Range(0, TandemToShuffle.Count + roomToShuffle2.Count - 1);
                    if (TandemToShuffle.Count > 0 && (rand2 == 0 || roomToShuffle2.Count == 0))
                    {
                        rand = Random.Range(0, TandemToShuffle.Count);
                        TandemToShuffle[rand].transform.position = roomPosition.Position;
                        TandemToShuffle[rand].RoomSide = Room.Side.RIGHT;
                        TandemToShuffle.Remove(TandemToShuffle[rand]);           
                    }
                    else
                    {
                        rand = Random.Range(0, roomToShuffle2.Count);
                        roomToShuffle2[rand].transform.position = roomPosition.Position;
                        roomToShuffle2[rand].RoomSide = Room.Side.RIGHT;
                        roomToShuffle2.Remove(roomToShuffle2[rand]);
                    }
                    break;
                case 2:
                    rand = Random.Range(0, roomToShuffle3.Count);
                    roomToShuffle3[rand].transform.position = roomPosition.Position;
                    roomToShuffle3[rand].RoomSide = Room.Side.RIGHT;
                    roomToShuffle3.Remove(roomToShuffle3[rand]);
                    break;
                case 3:
                    rand = Random.Range(0, roomToShuffle4.Count);
                    roomToShuffle4[rand].transform.position = roomPosition.Position;
                    roomToShuffle4[rand].RoomSide = Room.Side.RIGHT;
                    roomToShuffle4.Remove(roomToShuffle4[rand]);
                    break;
             }
            i++;
            if (i >= _roomsInPlay.Count / 2 + 1+ _maxRooms)
            {
                break;
            }
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
            room.name = "room "+room.Doors.Count+ " doors : " + room.transform.position;
            room.LinkedRooms.Clear();
            room.UsedDoormats.Clear();
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
    private void LinkRoom(Room room, Room roomToLink, Door door)
    {
        Material doorMat = _doormattMaterials[0];
        foreach(Material material in _doormattMaterials)
        {
            if (!room.UsedDoormats.Contains(material) && !roomToLink.UsedDoormats.Contains(material))
            {
                doorMat = material;
                break;
            }
        }
        foreach (Door doorToLink in roomToLink.Doors)
        {
            if (doorToLink.LinkedDoor == null)
            {
                room.UsedDoormats.Add(doorMat);
                roomToLink.UsedDoormats.Add(doorMat);
                door.DoormatMat = doorMat;
                doorToLink.DoormatMat = doorMat;
                room.LinkedRooms.Add(roomToLink);
               roomToLink.LinkedRooms.Add(room);
                door.LinkedDoor = doorToLink;
                doorToLink.LinkedDoor = door;
                door.UpdateDoormat();
                doorToLink.UpdateDoormat();
                break;
            }
        }
    }
    public void LockedDoor()
    {
        foreach(Room room in _roomsInPlay)
        {
            if (room.RoomSide != Room.Side.HUB)
            {
                foreach(Door door in room.Doors)
                {
                    door.IsLocked = true;
                }
            }
        }
        int validated = GameManager.Instance.ValidatedRooom;
        if (validated < 2)
            _maxRooms = 3;
        else if (validated < 5)
            _maxRooms = 5;
        else if (validated < 10)
            _maxRooms = _roomsInPlay.Count / 2;
        else if (GameManager.Instance.CurrentTimerPhase == GameManager.TimerPhase.END)
            _maxRooms = 0;
        //Debug.Log(_maxRooms);
        for (int i = 0; i < _maxRooms; i++)
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
    public void SetRoomsRewards(List<Clue> clues)
    {
        foreach (Clue clue in clues)
        {
            _rewards.Add(clue.gameObject);
            _rewardClueOnly.Add(clue.gameObject);
        }
        if (FindObjectsOfType<Statue>().Length > FindObjectsOfType<Item>().Length)
        {
            for (int i = 0; i < FindObjectsOfType<Statue>().Length - FindObjectsOfType<Item>().Length; i++)
            {
                _rewards.Add(arm);
            }
        }
        int k = 0;
        foreach (Room room2 in _roomsInPlay)
        {
            if (room2.RoomSide != Room.Side.HUB&&room2.CanHaveReward)
            {
                int rand = Random.Range(0, _roomsInPlay.Count-1 - k);
                if (rand < _rewards.Count && !room2.IsRewardClue)
                {
                    room2.Reward = _rewards[rand];
                    _rewards.Remove(_rewards[rand]);
                    if(rand < _rewardClueOnly.Count)
                    {
                        _rewardClueOnly.Remove(_rewardClueOnly[rand]);
                    }
                }
                else if (rand < _rewardClueOnly.Count)
                {
                    room2.Reward = _rewardClueOnly[rand];
                    _rewardClueOnly.Remove(_rewardClueOnly[rand]);
                    _rewards.Remove(_rewards[rand]);
                }
                k++;
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
