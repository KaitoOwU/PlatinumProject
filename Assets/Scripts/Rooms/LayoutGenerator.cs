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
<<<<<<< Updated upstream
    [SerializeField] List<RoomPosition> allAisleRoomL = new List<RoomPosition>();
    [SerializeField] List<RoomPosition> allAisleRoomR = new List<RoomPosition>();
=======

    public List<List<RoomPosition>> AisleLeft { get => _aisleLeft; }
    public List<List<RoomPosition>> AisleRight { get => _aisleRight;}
>>>>>>> Stashed changes

    // Start is called before the first frame update
    void Start()
    {
        _aisleLeft = new List<List<RoomPosition>>();
        _aisleRight = new List<List<RoomPosition>>();
<<<<<<< Updated upstream
        AisleLayout(_aisleLeft, true, _oneDoorRoomL, _twoDoorRoomL, _threeDoorRoomL, _fourDoorRoomL,allAisleRoomL);
        AisleLayout(_aisleRight, false, _oneDoorRoomR, _twoDoorRoomR, _threeDoorRoomR, _fourDoorRoomR,allAisleRoomR);
    }
    private void AisleLayout(List<List<RoomPosition>> aisle,bool isLeft, List<RoomPosition> oneDoorRoom, List<RoomPosition> twoDoorRoom, List<RoomPosition> threeDoorRoom, List<RoomPosition> fourDoorRoom,List<RoomPosition> allAisleRoom)
    {
        allAisleRoom = new List<RoomPosition>();
=======
        AisleLayout(_aisleLeft, true, _oneDoorRoomL, _twoDoorRoomL, _threeDoorRoomL, _fourDoorRoomL);
        AisleLayout(_aisleRight, false, _oneDoorRoomR, _twoDoorRoomR, _threeDoorRoomR, _fourDoorRoomR);
        FindObjectOfType<RoomGeneration>().GenerateRooms();
    }
    #region LayoutGeneration
    private void AisleLayout(List<List<RoomPosition>> aisle,bool isLeft, List<RoomPosition> oneDoorRoom, List<RoomPosition> twoDoorRoom, List<RoomPosition> threeDoorRoom, List<RoomPosition> fourDoorRoom)
    {
        List<RoomPosition> allAisleRoom = new List<RoomPosition>();
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            allAisleRoom.Add(new RoomPosition(new Vector3(_hub.transform.position.x - 30, _hub.transform.position.y, _hub.transform.position.z)));
        }
        else
        {
            allAisleRoom.Add(new RoomPosition(new Vector3(_hub.transform.position.x + 30, _hub.transform.position.y, _hub.transform.position.z)));
=======
            allAisleRoom.Add(new RoomPosition(new Vector3(_hub.transform.position.x - _betweenRoomDistance, _hub.transform.position.y, _hub.transform.position.z)));
        }
        else
        {
            allAisleRoom.Add(new RoomPosition(new Vector3(_hub.transform.position.x + _betweenRoomDistance, _hub.transform.position.y, _hub.transform.position.z)));
>>>>>>> Stashed changes
        }
        int i = 1;
        int count = allAisleRoom.Count;
        Debug.Log(allAisleRoom.Count);
        for (int  k= 0; k < 100;++k ) 
        {
            for (int j = 0; j < 4; j++)
            {
<<<<<<< Updated upstream
                if (allAisleRoom.Count - 1 > _sizeAisle)
=======
                if (allAisleRoom.Count - 1 >= _sizeAisle)
>>>>>>> Stashed changes
                    break;
                int rand = Random.Range(0, 2);
                if (rand == 1)
                {
                    switch (j)
                    {
                        case 0:
<<<<<<< Updated upstream
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[i].Position.x - 30, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z), allAisleRoom)&& allAisleRoom[i].Position.x - 30!=0)
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[i].Position.x - 30, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z)));
=======
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[i].Position.x - _betweenRoomDistance, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z), allAisleRoom)&& allAisleRoom[i].Position.x - _betweenRoomDistance!=0)
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[i].Position.x - _betweenRoomDistance, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z)));
>>>>>>> Stashed changes
                               Debug.Log(allAisleRoom[allAisleRoom.Count-1].Position + "1");
                                k++;
                            }
                            break;
                        case 1:
<<<<<<< Updated upstream
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[i].Position.x + 30, allAisleRoom[i ].Position.y, allAisleRoom[i].Position.z), allAisleRoom)&& allAisleRoom[i].Position.x + 30!=0)
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[i].Position.x + 30, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z)));
=======
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[i].Position.x + _betweenRoomDistance, allAisleRoom[i ].Position.y, allAisleRoom[i].Position.z), allAisleRoom)&& allAisleRoom[i].Position.x + _betweenRoomDistance!=0)
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[i].Position.x + _betweenRoomDistance, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z)));
>>>>>>> Stashed changes
                                Debug.Log(allAisleRoom[allAisleRoom.Count - 1].Position + "2");
                                k++;
                            }
                            break;
                        case 2:
<<<<<<< Updated upstream
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[i].Position.x, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z + 30), allAisleRoom))
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[i].Position.x , allAisleRoom[i].Position.y, allAisleRoom[i].Position.z + 30)));
=======
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[i].Position.x, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z + _betweenRoomDistance), allAisleRoom))
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[i].Position.x , allAisleRoom[i].Position.y, allAisleRoom[i].Position.z + _betweenRoomDistance)));
>>>>>>> Stashed changes
                                Debug.Log(allAisleRoom[allAisleRoom.Count - 1].Position + "3");
                                k++;
                            }
                            break;
                        case 3:
<<<<<<< Updated upstream
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[i].Position.x, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z - 30), allAisleRoom))
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[i].Position.x , allAisleRoom[i].Position.y, allAisleRoom[i].Position.z - 30)));
=======
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[i].Position.x, allAisleRoom[i].Position.y, allAisleRoom[i].Position.z - _betweenRoomDistance), allAisleRoom))
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[i].Position.x , allAisleRoom[i].Position.y, allAisleRoom[i].Position.z - _betweenRoomDistance)));
>>>>>>> Stashed changes
                                Debug.Log(allAisleRoom[allAisleRoom.Count - 1].Position + "4");
                                k++;
                            }
                            break;
                    }
                }
<<<<<<< Updated upstream
                if (allAisleRoom.Count - 1 > _sizeAisle )
=======
                if (allAisleRoom.Count - 1 >= _sizeAisle )
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            allAisleRoom[j].DoorCheck(allAisleRoom);
=======
            allAisleRoom[j].DoorCheck(allAisleRoom,_betweenRoomDistance);
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
    #endregion
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
[System.Serializable]
public class RoomPosition
{
   [SerializeField] private Vector3 _position;
    [SerializeField] private int _doorNumber;

    public Vector3 Position { get => _position; }
    public int DoorNumber { get => _doorNumber; }
=======

[System.Serializable]
public class RoomPosition
{ 
   [SerializeField] private Vector3 _position;
    [SerializeField] private int _doorNumber;
    [SerializeField] private List<Vector3> _nearbyPos = new List<Vector3>();

    public Vector3 Position { get => _position; }
    public int DoorNumber { get => _doorNumber; }
    public List<Vector3> NearbyPos { get => _nearbyPos; }
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
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
=======
    public void DoorCheck(List<RoomPosition> roomPositions,int betweenRoomDistance)
    {
        foreach(RoomPosition roomPosition in roomPositions)
        {
            if (this._position.x + betweenRoomDistance == roomPosition._position.x && this._position.z == roomPosition._position.z)
            {
                ++_doorNumber;
                _nearbyPos.Add(new Vector3(this._position.x + betweenRoomDistance, this._position.y, this._position.z));
            }           
            else if (this._position.x - betweenRoomDistance == roomPosition._position.x && this._position.z == roomPosition._position.z)
            {
                ++_doorNumber;
                _nearbyPos.Add(new Vector3(this._position.x - betweenRoomDistance, this._position.y, this._position.z));
            }
            else if (this._position.z + betweenRoomDistance == roomPosition._position.z && this._position.x == roomPosition._position.x)
            {
                ++_doorNumber;
                _nearbyPos.Add(new Vector3(this._position.x, this._position.y, this._position.z+betweenRoomDistance));
            }
            else if (this._position.z - betweenRoomDistance == roomPosition._position.z && this._position.x == roomPosition._position.x)
            {
                ++_doorNumber;
                _nearbyPos.Add(new Vector3(this._position.x, this._position.y, this._position.z-betweenRoomDistance));
>>>>>>> Stashed changes
            }
        }
    }
}
