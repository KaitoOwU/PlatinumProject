using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
[System.Serializable]
public class Door : Interactable
{
    [SerializeField] private Transform[] _tpPoint;
    [SerializeField] private DoorType _doorTypeValue;
    [SerializeField] private Door _linkedDoor;
    [SerializeField] private Room room;

    public Transform[] TpPoint => _tpPoint;
    public DoorType DoorTypeValue => _doorTypeValue;

    public Door LinkedDoor { get => _linkedDoor; set => _linkedDoor = value; }

    public enum DoorType
    {
        ENTRY,
        EXIT,
    }

    protected override void OnInteract(Player player)
    {
        if(player.CurrentRoom is Hub)
        {
            int count = GameManager.Instance.PlayerList.FindAll(player => player.PlayerController.IsInteractHeld).Count;
            
            Hub hub = (Hub)player.CurrentRoom;
            if (hub.RoomDoorLeft.PlayersInRange.Count >= 1 && hub.RoomDoorRight.PlayersInRange.Count >= 1 && count == 4) // POUR BUILD FINALE ==> ==4 !!!
            {
                GameManager.Instance.SwitchCameraState(GameManager.CameraState.SPLIT);
                GameManager.Instance.CurrentGamePhase = GameManager.GamePhase.GAME;

                //TP All players to next room depending on the door they're interacting with (after they all hold button)
                hub.RoomDoorLeft.TP_Players(hub.RoomDoorLeft.LinkedDoor.TpPoint);
                hub.RoomDoorRight.TP_Players(hub.RoomDoorRight.LinkedDoor.TpPoint);

                hub.RoomDoorLeft.UpdateRoom(hub.RoomDoorLeft.LinkedDoor.room);
                hub.RoomDoorRight.UpdateRoom(hub.RoomDoorRight.LinkedDoor.room);

                hub.RoomDoorLeft.TP_Camera(hub.RoomDoorLeft.LinkedDoor.room);
                hub.RoomDoorRight.TP_Camera(hub.RoomDoorRight.LinkedDoor.room);
                
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
                    _playersInRange.All(player => player.PlayerController.IsInteractHeld))
                {
                    TP_Players(_linkedDoor.TpPoint);
                    TP_Camera(_linkedDoor.room);
                    UpdateRoom(_linkedDoor.room);
                }
            } else if (room.RoomSide == Room.Side.LEFT)
            {
                if (GameManager.Instance.LeftPlayers.Count == _playersInRange.Count &&
                    _playersInRange.All(player => player.PlayerController.IsInteractHeld))
                {
                    TP_Players(_linkedDoor.TpPoint);
                    TP_Camera(_linkedDoor.room);
                    UpdateRoom(_linkedDoor.room);
                }
            }
        }
    }

    public void TP_Players(Transform[] tpPoint)//TP  tous les joueurs qui intéragissent avec this porte
    { 
        //GARDER EN MEMOIRE LE NOMBRE DE JOUEUR POUR SAVOIR COMBIEN IL EN FAUT POUR PASSER A LA SALLE SUIVANTE DANS CHAQUE BRANCHE
        foreach(Player p in _playersInRange) if (p.PlayerController.IsInteractHeld)
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
}
