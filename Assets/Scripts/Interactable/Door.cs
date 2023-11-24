using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.EventSystems.EventTrigger;
[System.Serializable]
public class Door : Interactable
{
    [HideInInspector] public UnityEvent OnChangeRoom;

    [SerializeField] private Transform[] _tpPoint;
    [SerializeField] private DoorType _doorTypeValue;
    [SerializeField] private Door _linkedDoor;
    [SerializeField] private Room room;
    [SerializeField] private bool _isLocked;
    List<Corridor> _corridors;
    [SerializeField] private MeshRenderer _doormat;
    private Material _doormatMat;

    public Transform[] TpPoint => _tpPoint;
    public DoorType DoorTypeValue => _doorTypeValue;

    public Door LinkedDoor { get => _linkedDoor; set => _linkedDoor = value; }
    public bool IsLocked { get => _isLocked; set => _isLocked = value; }
    public Room Room { get => room;}
    public Material DoormatMat { get => _doormatMat; set => _doormatMat = value; }

    private void Start()
    {
        _corridors= FindObjectsOfType<Corridor>().ToList();
        room = GetComponentInParent<Room>();
    }
    public enum DoorType
    {
        ENTRY,
        EXIT,
    }

    protected override void OnInteract(Player player)
    {
        if (GameManager.Instance.CurrentGamePhase == GameManager.GamePhase.SELECT_CHARACTER)
            return;
        Debug.Log(player);
        if (!_isLocked && !_linkedDoor.IsLocked)
        {
            OnChangeRoom?.Invoke();

            if (player.CurrentRoom is Hub)
            {
                int countInHub = GameManager.Instance.PlayerList.FindAll(player => player.PlayerRef.RelativePos == HubRelativePosition.HUB).Count;
                int count = GameManager.Instance.PlayerList.FindAll(player => player.PlayerController.IsButtonHeld(PlayerController.EButtonType.INTERACT)).Count;

                Hub hub = (Hub)player.CurrentRoom;
                if (hub.RoomDoorLeft.PlayersInRange.Count >= 1 /*&& hub.RoomDoorRight.PlayersInRange.Count >= 1 && count == 4*/) // POUR BUILD FINALE ==> ==4 !!!
                {
                    GameManager.Instance.SwitchCameraState(GameManager.CameraState.SPLIT);
                    GameManager.Instance.CurrentGamePhase = GameManager.GamePhase.GAME;

                    hub.RoomDoorLeft.TP_Players(hub.RoomDoorLeft.LinkedDoor.TpPoint);
                    hub.RoomDoorRight.TP_Players(hub.RoomDoorRight.LinkedDoor.TpPoint);

                    hub.RoomDoorLeft.UpdateRoom(hub.RoomDoorLeft.LinkedDoor.room);
                    hub.RoomDoorRight.UpdateRoom(hub.RoomDoorRight.LinkedDoor.room);

                    hub.RoomDoorLeft.TP_Camera(hub.RoomDoorLeft.LinkedDoor.room);
                    hub.RoomDoorRight.TP_Camera(hub.RoomDoorRight.LinkedDoor.room);

   
                }
                else if (_playersInRange.Count == countInHub && countInHub < 4)
                {
                    TP_Players(LinkedDoor.TpPoint);
                    TP_Camera(_linkedDoor.room);
                    UpdateRoom(_linkedDoor.room);

                    hub.RoomDoorLeft._isLocked = false;
                    hub.RoomDoorRight._isLocked = false;
                }
                else if(_linkedDoor.room is Vestibule)
                {
                    TP_Players(LinkedDoor.TpPoint);
                    TP_Camera(_linkedDoor.room);
                    UpdateRoom(_linkedDoor.room);

                    _isLocked = true;
                    GameManager.Instance.CurrentGamePhase = GameManager.GamePhase.GUESS;
                }
            }
            else
            {
                Room room = player.CurrentRoom;

                if (_linkedDoor.room is Hub)
                {
                    GameManager.Instance.OnBackToHubRefused?.Invoke(this);
                    return;
                }
                if (room.RoomSide == Room.Side.RIGHT)
                {
                    if (GameManager.Instance.RightPlayers.Count == _playersInRange.Count &&
                        _playersInRange.All(player => player.PlayerController.IsButtonHeld(PlayerController.EButtonType.INTERACT)))
                    {
                        TP_SidePlayers();
                    }
                }
                else if (room.RoomSide == Room.Side.LEFT)
                {
                    if (GameManager.Instance.LeftPlayers.Count == _playersInRange.Count &&
                        _playersInRange.All(player => player.PlayerController.IsButtonHeld(PlayerController.EButtonType.INTERACT)))
                    {
                        TP_SidePlayers();
                    }
                }
            }
        }
    }

    public void TP_Players(Transform[] tpPoint) // TP  tous les joueurs qui intéragissent avec this porte
    {
        foreach(Player p in _playersInRange) if (p.PlayerController.IsButtonHeld(PlayerController.EButtonType.INTERACT))
        {
            p.gameObject.transform.position = tpPoint[p.Index-1].position;
        }
    }

    public void TP_Camera(Room room)
    {
        switch (room.RoomSide)
        {
            case Room.Side.LEFT:
                GameManager.Instance.TP_LeftCamera(room.CameraPoint);
                break;
            case Room.Side.RIGHT:
                GameManager.Instance.TP_RightCamera(room.CameraPoint);
                break;
            case Room.Side.VESTIBULE:
                GameManager.Instance.TP_Camera(GameManager.Instance.FullCamera, room.CameraPoint);
                break;
        }
    }
    
    public void UpdateRoom(Room room)
    {
        foreach(Player p in _playersInRange)
        {
            if (p.CurrentRoom is Hub)
            {
                if (room.RoomSide == Room.Side.RIGHT)
                {
                    p.RelativePos =
                        HubRelativePosition.RIGHT_WING;
                }
                else if(room.RoomSide == Room.Side.LEFT)
                {
                    p.RelativePos =
                        HubRelativePosition.LEFT_WING;
                }
            }
            p.CurrentRoom = room;
        }
    }
    private void TP_SidePlayers()
    {
        int rand = Random.Range(0, 10);
        if (rand > GameManager.Instance.CorridorChance)
        {
            int rand2 = Random.Range(0, _corridors.Count);
            Corridor corridor = _corridors[rand2];
            corridor.SetCorridor(_playersInRange[0], LinkedDoor);
            TP_Players(corridor.Doors[0].TpPoint);
            TP_Camera(corridor);
            UpdateRoom(corridor);
            GameManager.Instance.CorridorChance = 10;
        }
        else
        {
            GameManager.Instance.CorridorChance--;
            TP_Players(_linkedDoor.TpPoint);
            TP_Camera(_linkedDoor.room);
            UpdateRoom(_linkedDoor.room);
        }
    }
    public void UpdateDoormat()
    {
        _doormat.material = _doormatMat;
    }
}
