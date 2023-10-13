using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Door : Interactable
{
    [SerializeField] private Transform _tpPoint;
    [SerializeField] private DoorType _doorTypeValue;

    public DoorType DoorTypeValue => _doorTypeValue;
    public enum DoorType
    {
        ENTRY,
        EXIT,
    }

    protected override void OnInteract(Player player)
    {
        Debug.Log(gameObject.name + " interacted with");

        switch (GameManager.Instance.CurrentGamePhase)
        {
            default:
                return;
            case GameManager.GamePhase.HUB:
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
                    hub.RoomDoorLeft.TP_Players();
                    hub.RoomDoorRight.TP_Players();

                    hub.RoomDoorLeft.TP_Camera(hub.RoomLeft);
                    hub.RoomDoorRight.TP_Camera(hub.RoomRight);
                }
                return;
        }
    }
    public void TP_Players()
    {
        foreach(Player p in _playersInRange)
        {
            p.gameObject.transform.position = _tpPoint.position;
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
        }
    }

}
