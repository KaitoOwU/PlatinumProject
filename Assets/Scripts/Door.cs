using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static UnityEngine.EventSystems.EventTrigger;
[System.Serializable]
public class Door : Interactable
{
    [SerializeField] private Transform _tpPoint;
    [SerializeField] private DoorType _doorTypeValue;
    [SerializeField] private Door _linkedDoor;
    [SerializeField] private Room room;

    public Transform TpPoint => _tpPoint;
    public DoorType DoorTypeValue => _doorTypeValue;

    public Door LinkedDoor { get => _linkedDoor; set => _linkedDoor = value; }

    public enum DoorType
    {
        ENTRY,
        EXIT,
    }

    /*protected override void OnInteract(Player player)
    {
        if(player.CurrentRoom is Hub)
        {
            int count = 0;
            foreach (PlayerInfo p in GameManager.Instance.PlayerList)
            {
                if(p.PlayerController.IsInteractHeld) 
                    count++;
            }
            if (count == 2) // 2 POUR TEST ==> 4 !!!
            {
                Hub hub = (Hub)player.CurrentRoom;
                GameManager.Instance.SwitchCameraState(CameraState.SPLIT);
                GameManager.Instance.CurrentGamePhase = GameManager.GamePhase.GAME;

                //TP All players to next room depending on the door they're interacting with (after they all hold button)
                hub.RoomDoorLeft.TP_Players(hub.RoomLeft.PreviousRoomDoor.TpPoint);
                hub.RoomDoorRight.TP_Players(hub.RoomRight.PreviousRoomDoor.TpPoint);

                hub.RoomDoorLeft.UpdateRoom(hub.RoomLeft);
                hub.RoomDoorRight.UpdateRoom(hub.RoomRight);

                hub.RoomDoorLeft.TP_Camera(hub.RoomLeft);
                hub.RoomDoorRight.TP_Camera(hub.RoomRight);
                
            }
        }
        else
        {
            Room room = player.CurrentRoom;
            Debug.Log("PLAYER  "+ player.Index );
            switch (DoorTypeValue)
            {
                case DoorType.ENTRY:
                    //SI TU RETOURNES AU HUB
                    if(room.PreviousRoom is Hub)
                    {
                        Hub hub = (Hub)room.PreviousRoom;
                        if(room.RoomSide == Hub.Side.LEFT)
                            TP_Players(hub.RoomDoorLeft.TpPoint);
                        else
                            TP_Players(hub.RoomDoorRight.TpPoint);
                        TP_Camera(room.PreviousRoom);
                        UpdateRoom(room.PreviousRoom);
                    }
                    else
                    {
                        TP_Players(room.PreviousRoom.NextRoomDoor.TpPoint);
                        TP_Camera(room.PreviousRoom);
                        UpdateRoom(room.PreviousRoom);
                    }
                    break;
                case DoorType.EXIT:
                    TP_Players(room.NextRoom.PreviousRoomDoor.TpPoint);
                    TP_Camera(room.NextRoom);
                    UpdateRoom(room.NextRoom);
                    break;
            }                     
        }
    }*/

    public void TP_Players(Transform tpPoint)//TP  tous les joueurs qui int�ragissent avec this porte
    { 
        //GARDER EN MEMOIRE LE NOMBRE DE JOUEUR POUR SAVOIR COMBIEN IL EN FAUT POUR PASSER A LA SALLE SUIVANTE DANS CHAQUE BRANCHE
        foreach(Player p in _playersInRange)
        {
            p.gameObject.transform.position = tpPoint.position;
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
            p.CurrentRoom = room;
        }
    }
}
