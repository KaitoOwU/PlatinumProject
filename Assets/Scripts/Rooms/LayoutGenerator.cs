using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{
    private List<List<RoomPosition>> _aisleLeft;
    private List<List<RoomPosition>> _aisleRight;
    private List<RoomPosition>_aisleLeftInOrder;
    private List<RoomPosition> _aisleRightInOrder;
    [SerializeField] private Hub _hub;
    [SerializeField] private int _sizeAisle;
    [SerializeField] private int _betweenRoomDistance;
    private List<Vector3> _lineListLayout = new List<Vector3>();
    private List<Vector3> _lineListAddedCorridor= new List<Vector3>();
    private Vector3[] _pointsLayout;
    private Vector3[] _pointsAddedCorridor;

    public List<List<RoomPosition>> AisleLeft { get => _aisleLeft; }
    public List<List<RoomPosition>> AisleRight { get => _aisleRight;}
    public int BetweenRoomDistance { get => _betweenRoomDistance; }
    public List<RoomPosition> AisleLeftInOrder { get => _aisleLeftInOrder; }
    public List<RoomPosition> AisleRightInOrder { get => _aisleRightInOrder;}

    // Start is called before the first frame update
    void Awake()
    {
        _aisleLeft = new List<List<RoomPosition>>();
        _aisleRight = new List<List<RoomPosition>>();
        _aisleLeftInOrder = new List<RoomPosition>();
        _aisleRightInOrder = new List<RoomPosition>();
        AisleLayout(_aisleLeft, true,AisleLeftInOrder);
        AisleLayout(_aisleRight, false,AisleRightInOrder);
        _pointsLayout = new Vector3[_lineListLayout.Count];
        _pointsAddedCorridor = new Vector3[_lineListAddedCorridor.Count];
        for(int i = 0; i < _lineListLayout.Count; i++)
        {
            _pointsLayout[i] = _lineListLayout[i];
        }
        for(int i = 0;i< _lineListAddedCorridor.Count; i++)
        {
            _pointsAddedCorridor[i] = _lineListAddedCorridor[i];
        }
        StartCoroutine(FindObjectOfType<RoomGeneration>().GenerateRooms());
    }
    #region LayoutGeneration
    private void AisleLayout(List<List<RoomPosition>> aisle,bool isLeft,List<RoomPosition> aisleInOrder)
    {
        List<RoomPosition> allAisleRoom = new List<RoomPosition>();
        List<RoomPosition> oneDoorRoom = new List<RoomPosition>();
        List<RoomPosition> twoDoorRoom = new List<RoomPosition>();
        List<RoomPosition> threeDoorRoom = new List<RoomPosition>();
        List<RoomPosition> fourDoorRoom = new List<RoomPosition>();
        RoomPosition hubPosition;
        //Debug.Log(_hub.name);
        hubPosition = new RoomPosition(_hub.transform.position, 1);
        allAisleRoom.Add(hubPosition);
        _lineListLayout.Add(hubPosition.Position);
        if (isLeft)
        {
            allAisleRoom.Add(new RoomPosition(new Vector3(_hub.transform.position.x - _betweenRoomDistance, _hub.transform.position.y, _hub.transform.position.z)));
            _lineListLayout.Add(new Vector3(_hub.transform.position.x - _betweenRoomDistance, _hub.transform.position.y, _hub.transform.position.z));
        }
        else
        {
            allAisleRoom.Add(new RoomPosition(new Vector3(_hub.transform.position.x + _betweenRoomDistance, _hub.transform.position.y, _hub.transform.position.z)));
            _lineListLayout.Add(new Vector3(_hub.transform.position.x + _betweenRoomDistance, _hub.transform.position.y, _hub.transform.position.z));
        }
        for (int  i= 0; i < _sizeAisle-1;i++ ) 
        {
            for (int j = 0; j < 4; j++)
            {
                if (allAisleRoom.Count - 1 >= _sizeAisle)
                    break;
                int rand = Random.Range(0, 2);
                int rand2 = Random.Range(1, allAisleRoom.Count - 1);
                int k = 0;
                if (rand == 1)
                {
                    switch (j)
                    {
                        case 0:
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[rand2].Position.x - _betweenRoomDistance, allAisleRoom[rand2].Position.y, allAisleRoom[rand2].Position.z), allAisleRoom)&& allAisleRoom[rand2].Position.x - _betweenRoomDistance!=0)
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[rand2].Position.x - _betweenRoomDistance, allAisleRoom[rand2].Position.y, allAisleRoom[rand2].Position.z)));
                                _lineListLayout.Add(new Vector3(allAisleRoom[rand2].Position.x - _betweenRoomDistance, allAisleRoom[rand2].Position.y, allAisleRoom[rand2].Position.z));
                                _lineListLayout.Add(allAisleRoom[rand2].Position);
                                k++;
                            }
                            break;
                        case 1:
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[rand2].Position.x + _betweenRoomDistance, allAisleRoom[rand2].Position.y, allAisleRoom[rand2].Position.z), allAisleRoom)&& allAisleRoom[rand2].Position.x + _betweenRoomDistance!=0)
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[rand2].Position.x + _betweenRoomDistance, allAisleRoom[rand2].Position.y, allAisleRoom[rand2].Position.z)));
                                _lineListLayout.Add(new Vector3(allAisleRoom[rand2].Position.x + _betweenRoomDistance, allAisleRoom[rand2].Position.y, allAisleRoom[rand2].Position.z));
                                _lineListLayout.Add(allAisleRoom[rand2].Position);
                                k++;
                            }
                            break;
                        case 2:
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[rand2].Position.x, allAisleRoom[rand2].Position.y, allAisleRoom[rand2].Position.z + _betweenRoomDistance), allAisleRoom))
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[rand2].Position.x , allAisleRoom[rand2].Position.y, allAisleRoom[rand2].Position.z + _betweenRoomDistance)));
                                _lineListLayout.Add(new Vector3(allAisleRoom[rand2].Position.x , allAisleRoom[rand2].Position.y, allAisleRoom[rand2].Position.z + _betweenRoomDistance));
                                _lineListLayout.Add(allAisleRoom[rand2].Position);
                                k++;
                            }
                            break;
                        case 3:
                            if (!IsRoomPositionTaken(new Vector3(allAisleRoom[rand2].Position.x, allAisleRoom[rand2].Position.y, allAisleRoom[rand2].Position.z - _betweenRoomDistance), allAisleRoom))
                            {
                                allAisleRoom.Add(new RoomPosition(new Vector3(allAisleRoom[rand2].Position.x , allAisleRoom[rand2].Position.y, allAisleRoom[rand2].Position.z - _betweenRoomDistance)));
                                _lineListLayout.Add(new Vector3(allAisleRoom[rand2].Position.x, allAisleRoom[rand2].Position.y, allAisleRoom[rand2].Position.z - _betweenRoomDistance));
                                _lineListLayout.Add(allAisleRoom[rand2].Position);
                                k++;
                            }
                            break;
                    }
                }
                if (k == 0)
                {
                    i--;
                }
                if (allAisleRoom.Count - 1 >= _sizeAisle )
                    break; 
            }      
        }
        for (int j = 1; j < allAisleRoom.Count; ++j)
        {
            allAisleRoom[j].DoorCheck(allAisleRoom,_betweenRoomDistance,_lineListLayout,_lineListAddedCorridor);
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
            aisleInOrder.Add(allAisleRoom[j]);
        }
        aisle.Add(oneDoorRoom);
        aisle.Add(twoDoorRoom);
        aisle.Add(threeDoorRoom);
        aisle.Add(fourDoorRoom);
    }
    #endregion
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
    private void OnDrawGizmos()
    {
        Gizmos.DrawLineList(_pointsAddedCorridor);
        Gizmos.color = Color.green;
        Gizmos.DrawLineList(_pointsLayout);
    }
}

public class RoomPosition
{ 
    private Vector3 _position;
    private int _doorNumber;

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

    public void DoorCheck(List<RoomPosition> roomPositions,int betweenRoomDistance,List<Vector3> lineListLayout, List<Vector3> lineListAddedCorridor)
    {
        foreach(RoomPosition roomPosition in roomPositions)
        {
            if (this._position.x + betweenRoomDistance == roomPosition._position.x && this._position.z == roomPosition._position.z)
            {
                lineListAddedCorridor.Add(Position);
                ++_doorNumber;
                lineListAddedCorridor.Add(new Vector3(this._position.x + betweenRoomDistance, this._position.y, this._position.z));
            }           
            else if (this._position.x - betweenRoomDistance == roomPosition._position.x && this._position.z == roomPosition._position.z)
            {
                lineListAddedCorridor.Add(Position);
                ++_doorNumber;
                lineListAddedCorridor.Add(new Vector3(this._position.x - betweenRoomDistance, this._position.y, this._position.z));
            }
            else if (this._position.z + betweenRoomDistance == roomPosition._position.z && this._position.x == roomPosition._position.x)
            {
                lineListAddedCorridor.Add(Position);
                ++_doorNumber;
                lineListAddedCorridor.Add(new Vector3(this._position.x, this._position.y, this._position.z + betweenRoomDistance));
            }
            else if (this._position.z - betweenRoomDistance == roomPosition._position.z && this._position.x == roomPosition._position.x)
            {
                lineListAddedCorridor.Add(Position);
                ++_doorNumber;
                lineListAddedCorridor.Add(new Vector3(this._position.x, this._position.y, this._position.z - betweenRoomDistance));
            }
        }
    }
}
