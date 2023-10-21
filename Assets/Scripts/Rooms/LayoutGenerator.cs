using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{
    private List<List<RoomPosition>> _aisleLeft;
    private List<List<RoomPosition>> _aisleRight;
    [SerializeField] private Hub _hub;
    [SerializeField] private int _sizeAisle;
    [SerializeField] private int _betweenRoomDistance;
    [SerializeField] List<RoomPosition>_oneDoorRoomL = new List<RoomPosition>();
    [SerializeField] List<RoomPosition>_twoDoorRoomL = new List<RoomPosition>();
    [SerializeField] List<RoomPosition> _threeDoorRoomL = new List<RoomPosition>();
    [SerializeField] List<RoomPosition> _fourDoorRoomL = new List<RoomPosition>();
    [SerializeField] List<RoomPosition> _oneDoorRoomR = new List<RoomPosition>();
    [SerializeField] List<RoomPosition> _twoDoorRoomR= new List<RoomPosition>();
    [SerializeField] List<RoomPosition> _threeDoorRoomR = new List<RoomPosition>();
    [SerializeField] List<RoomPosition> _fourDoorRoomR = new List<RoomPosition>();
    [SerializeField] List<RoomPosition> allAisleRoomL = new List<RoomPosition>();
    [SerializeField] List<RoomPosition> allAisleRoomR = new List<RoomPosition>();

    // Start is called before the first frame update
    void Start()
    {
        _aisleLeft = new List<List<RoomPosition>>();
        _aisleRight = new List<List<RoomPosition>>();
        AisleLayout(_aisleLeft, true, _oneDoorRoomL, _twoDoorRoomL, _threeDoorRoomL, _fourDoorRoomL,allAisleRoomL);
        AisleLayout(_aisleRight, false, _oneDoorRoomR, _twoDoorRoomR, _threeDoorRoomR, _fourDoorRoomR,allAisleRoomR);
    }
    private void AisleLayout(List<List<RoomPosition>> aisle,bool isLeft, List<RoomPosition> oneDoorRoom, List<RoomPosition> twoDoorRoom, List<RoomPosition> threeDoorRoom, List<RoomPosition> fourDoorRoom,List<RoomPosition> allAisleRoom)
    {
        allAisleRoom = new List<RoomPosition>();
        RoomPosition _hubPosition;
        if (isLeft)
        {
            _hubPosition = new RoomPosition(_hub.transform.position, 1);
        }
        else
        {
            _hubPosition = new RoomPosition(_hub.transform.position, 1);
        }
        allAisleRoom.Add(_hubPosition);
        
        if (isLeft)
        {
            allAisleRoom.Add(new RoomPosition(new Vector3(_hub.transform.position.x - 30, _hub.transform.position.y, _hub.transform.position.z)));
        }
        else
        {
            allAisleRoom.Add(new RoomPosition(new Vector3(_hub.transform.position.x + 30, _hub.transform.position.y, _hub.transform.position.z)));
        }
        int i = 1;
        int count = allAisleRoom.Count;
        Debug.Log(allAisleRoom.Count);
        for (int  k= 0; k < 100;++k ) 
        {
            for (int j = 0; j < 4; j++)
            {
                if (allAisleRoom.Count - 1 > _sizeAisle)
                    break;
                int rand = Random.Range(0, 2);
                if (rand == 1)
                {
                    switch (j)
                    {
                        case 0:
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[i].Position.x - 30, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z), allAisleRoom)&& allAisleRoom[i].Position.x - 30!=0)
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[i].Position.x - 30, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z)));
                               Debug.Log(allAisleRoom[allAisleRoom.Count-1].Position + "1");
                                k++;
                            }
                            break;
                        case 1:
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[i].Position.x + 30, allAisleRoom[i ].Position.y, allAisleRoom[i].Position.z), allAisleRoom)&& allAisleRoom[i].Position.x + 30!=0)
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[i].Position.x + 30, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z)));
                                Debug.Log(allAisleRoom[allAisleRoom.Count - 1].Position + "2");
                                k++;
                            }
                            break;
                        case 2:
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[i].Position.x, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z + 30), allAisleRoom))
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[i].Position.x , allAisleRoom[i].Position.y, allAisleRoom[i].Position.z + 30)));
                                Debug.Log(allAisleRoom[allAisleRoom.Count - 1].Position + "3");
                                k++;
                            }
                            break;
                        case 3:
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[i].Position.x, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z - 30), allAisleRoom))
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[i].Position.x , allAisleRoom[i].Position.y, allAisleRoom[i].Position.z - 30)));
                                Debug.Log(allAisleRoom[allAisleRoom.Count - 1].Position + "4");
                                k++;
                            }
                            break;
                    }
                }
                if (allAisleRoom.Count - 1 > _sizeAisle )
                    break;
                if (count < allAisleRoom.Count)
                {
                    i ++;
                    count = allAisleRoom.Count;
                }
               
                
            }
            
        }
        for (int j = 1; j < allAisleRoom.Count; ++j)
        {
            allAisleRoom[j].DoorCheck(allAisleRoom);
            switch (allAisleRoom[j].DoorNumber)
            {
                case 1:
                    oneDoorRoom.Add(allAisleRoom[j]);
                    break;
                case 2:
                    twoDoorRoom.Add(allAisleRoom[j]);
                    break;
                case 3:
                    threeDoorRoom.Add(allAisleRoom[j]);
                    break;
                case 4:
                    fourDoorRoom.Add(allAisleRoom[j]);
                    break;
            }
        }
        aisle.Add(oneDoorRoom);
        aisle.Add(twoDoorRoom);
        aisle.Add(threeDoorRoom);
        aisle.Add(fourDoorRoom);
    }
    public bool IsRoomPositionTaken(Vector3 newPosition, List<RoomPosition> roomPositions)
    {
        bool isTaken = false;
        foreach (RoomPosition roomPosition in roomPositions)
        {
            if (newPosition == roomPosition.Position)
            {
                isTaken = true;
            }
        }
        return isTaken;
    }
}
[System.Serializable]
public class RoomPosition
{
   [SerializeField] private Vector3 _position;
    [SerializeField] private int _doorNumber;

    public Vector3 Position { get => _position; }
    public int DoorNumber { get => _doorNumber; }

    public RoomPosition (Vector3 position,int doorNumber)
    {
        _position = position;
        _doorNumber = doorNumber;
    }
    public RoomPosition (Vector3 position)
    {
        _position = position;
        _doorNumber = 0;
    }
    public void DoorCheck(List<RoomPosition> roomPositions)
    {
        foreach(RoomPosition roomPosition in roomPositions)
        {
            if (this._position.x + 30 == roomPosition._position.x && this._position.z == roomPosition._position.z)
                ++_doorNumber;
            else if (this._position.x - 30 == roomPosition._position.x && this._position.z == roomPosition._position.z)
            {
                ++_doorNumber;
            }
            else if (this._position.z + 30 == roomPosition._position.z && this._position.x == roomPosition._position.x)
            {
                ++_doorNumber;
            }
            else if (this._position.z - 30 == roomPosition._position.z && this._position.x == roomPosition._position.x)
            {
                ++_doorNumber;
            }
        }
    }
}
