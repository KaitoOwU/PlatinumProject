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
    List<GameObject> _corridors;

    public Transform[] TpPoint => _tpPoint;
    public DoorType DoorTypeValue => _doorTypeValue;

    public Door LinkedDoor { get => _linkedDoor; set => _linkedDoor = value; }
    public bool IsLocked { get => _isLocked; set => _isLocked = value; }
    public Room Room { get => room;}

    private void Start()
    {
        _corridors= Resources.Load<SCRoomsLists>("ScriptableObject/Rooms").Floors[4].Rooms;
    }
    public enum DoorType
    {
        ENTRY,
        EXIT,
    }

    protected override void OnInteract(Player player)
    {
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

                    hub.RoomDoorLeft._isLocked = true;
                    hub.RoomDoorRight._isLocked = true;
                }
                else if (_playersInRange.Count == countInHub && countInHub < 4)
                {
                    TP_Players(LinkedDoor.TpPoint);
                    TP_Camera(_linkedDoor.room);
                    UpdateRoom(_linkedDoor.room);

                    _isLocked = true;
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

    public void TP_Camera(Room room) // Est-ce que tu bouges vers une room de l'aile gauche? alors tp cam de gauche
    {
        switch (room.RoomSide)
        {
            case Room.Side.LEFT:
                GameManager.Instance.TP_LeftCamera(room.CameraPoint);
                break;
            case Room.Side.RIGHT:
                GameManager.Instance.TP_RightCamera(room.CameraPoint);
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

        if (rand < GameManager.Instance.CorridorChance)
        {
            int rand2 = Random.Range(0, _corridors.Count);
            Corridor corridor = _corridors[rand2].GetComponent<Corridor>();
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
}
