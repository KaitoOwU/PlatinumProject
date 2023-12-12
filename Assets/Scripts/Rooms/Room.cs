using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class Room : MonoBehaviour
{

    /*[SerializeField] private Room _nextRoom;
    [SerializeField] private Room _previousRoom;

    [SerializeField] private Door _nextRoomDoor;
    [SerializeField] private Door _previousRoomDoor;*/
    [HideInInspector] public UnityEvent OnCompletedRoom;
    [HideInInspector] public UnityEvent OnUnlocked;
    [HideInInspector] public UnityEvent OnNewRoom;

    [SerializeField] private  List<Door> _doors =new List<Door>();
    [SerializeField] private List<Room> _linkedRooms = new List<Room>();
    [SerializeField] private Side _roomSide;
    [SerializeField] private Room _tandem;
    [SerializeField] private GameObject _reward;
    [SerializeField] protected Transform _cameraPoint;
    [SerializeField] private bool _isRewardClue;
    [SerializeField] private bool _canHaveReward;
    [SerializeField] private bool _discovered;
    private List<Material> _usedDoormats = new List<Material>();
    public enum Side
    {
        LEFT,
        RIGHT,
        HUB,
        CORRIDOR,
        VESTIBULE
    }

    /*public Room NextRoom { get => _nextRoom; set => _nextRoom = value; }
    public Room PreviousRoom { get => _previousRoom; set => _previousRoom = value; }    
    
    public Door NextRoomDoor { get => _nextRoomDoor; set => _nextRoomDoor = value; }
    public Door PreviousRoomDoor { get => _previousRoomDoor; set => _previousRoomDoor = value; }*/
    public Side RoomSide { get => _roomSide; set => _roomSide = value; }
    public Transform CameraPoint { get => _cameraPoint; set => _cameraPoint = value; }
    public List<Door> Doors { get => _doors; set => _doors = value; }
    public List<Room> LinkedRooms { get => _linkedRooms; set => _linkedRooms = value; }
    public Room Tandem { get => _tandem; set => _tandem = value; }
    public GameObject Reward { get => _reward; set => _reward = value; }
    public bool IsRewardClue { get => _isRewardClue; }
    public bool CanHaveReward { get => _canHaveReward; }
    public List<Material> UsedDoormats { get => _usedDoormats; set => _usedDoormats = value; }

    private void Awake()
    {
        Doors = GetComponentsInChildren<Door>().ToList();
    }
    public void OnSetUp()
    {
        if (!_doors[0].IsLocked && !_canHaveReward && (RoomSide != Side.HUB && RoomSide != Side.CORRIDOR))
        {
            Debug.Log(GameManager.Instance.ValidatedRooom);
            GameManager.Instance.ValidatedRooom++;
        }
    }
    public void CompletedRoom()
    {
        GameManager.Instance.ValidatedRooom++;
        Debug.Log(GameManager.Instance.ValidatedRooom);
        OnCompletedRoom?.Invoke();
        FindObjectOfType<RoomGeneration>().LockedDoor();
        if(GameManager.Instance.ValidatedRooom==6|| GameManager.Instance.ValidatedRooom == 10)
        {
            OnUnlocked?.Invoke();
        }
    }
    public void EnterRoom()
    {
        if (!_discovered)
        {
            _discovered = true;
            OnNewRoom?.Invoke();
        }
    }
    public int PlayerInRoom()
    {
        int pInRoom = 0;
        foreach (Player p in FindObjectsOfType<Player>().ToList().FindAll(player => player.CurrentRoom == this))
        {
            pInRoom++;
        }
        return pInRoom;
    }
}
