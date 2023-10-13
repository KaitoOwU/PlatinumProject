using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Door : Interactable
{
    [SerializeField] Transform _tpPoint;
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
                    //TP cameras to camera point in next room !!!
                    GameManager.Instance.SwitchCameraState(CameraState.SPLIT);
                    GameManager.Instance.CurrentGamePhase = GameManager.GamePhase.GAME;
                    Door[] doors = FindObjectsOfType<Door>(); // A FAIRE AVEC INFOS DE ROOM
                    foreach (Door d in doors)
                        d.TP_Players();
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

}
