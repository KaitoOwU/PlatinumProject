using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static CameraBehaviour;
[System.Serializable]
public class Door : Interactable
{

    [HideInInspector] public UnityEvent OnChangeRoom;
    [HideInInspector] public UnityEvent OnLockedDoorInterract;

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
        StartCoroutine(DoorInteraction(player));
    }

    private IEnumerator DoorInteraction(Player player)
    {
        if (!_isLocked && !_linkedDoor.IsLocked)
        {
            
            OnChangeRoom?.Invoke();
            if (player.CurrentRoom is Hub) // IF PLAYERS IN HUB
            {
                //
                GameManager.Instance.SplitCameraLeftBehaviour.ChangeCameraState(ECameraBehaviourState.STILL, _playersInRange.Select(p => p.gameObject).ToArray());
                GameManager.Instance.SplitCameraRightBehaviour.ChangeCameraState(ECameraBehaviourState.STILL, _playersInRange.Select(p => p.gameObject).ToArray());

#if UNITY_EDITOR
#else
                var playersInRoom = GameManager.Instance.PlayerList.FindAll(p => p.PlayerRef.CurrentRoom is Hub).ToList();
                if (playersInRoom.FindAll(player => player.PlayerController.IsButtonHeld(PlayerController.EButtonType.INTERACT)).Count != playersInRoom.Count)
                    yield break;
#endif

                int countInHub = GameManager.Instance.PlayerList.FindAll(player => player.PlayerRef.RelativePos == HubRelativePosition.HUB).Count;
                int count = GameManager.Instance.PlayerList.FindAll(player => player.PlayerController.IsButtonHeld(PlayerController.EButtonType.INTERACT)).Count;

                Hub hub = (Hub)player.CurrentRoom;
                
                #if UNITY_EDITOR
                if ((hub.RoomDoorRight.PlayersInRange.Count >= 1 || hub.RoomDoorLeft.PlayersInRange.Count >= 1) && count >= 1 && countInHub == 4)
                {
#else
                if (hub.RoomDoorRight.PlayersInRange.Count >= 1 && hub.RoomDoorLeft.PlayersInRange.Count >= 1 && count == 4  && countInHub == 4)
                {
#endif

                    GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = true);
                    yield return StartCoroutine(
                    UIRoomTransition.current.StartTransition(UIRoomTransition.current.HubTransition));


                    GameManager.Instance.SwitchCameraState(GameManager.CameraState.SPLIT);
                    GameManager.Instance.CurrentGamePhase = GameManager.GamePhase.GAME;

                    hub.RoomDoorLeft.TP_Players(hub.RoomDoorLeft.LinkedDoor.TpPoint);
                    hub.RoomDoorRight.TP_Players(hub.RoomDoorRight.LinkedDoor.TpPoint);

                    hub.RoomDoorLeft.UpdateRoom(hub.RoomDoorLeft.LinkedDoor.room);
                    hub.RoomDoorRight.UpdateRoom(hub.RoomDoorRight.LinkedDoor.room);

                    hub.RoomDoorLeft.TP_Camera(hub.RoomDoorLeft.LinkedDoor.room);
                    hub.RoomDoorRight.TP_Camera(hub.RoomDoorRight.LinkedDoor.room);

                    yield return StartCoroutine(
                        UIRoomTransition.current.EndTransition(UIRoomTransition.current.HubTransition));
                    GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);
   
                }
                else if (_playersInRange.Count == countInHub && countInHub < 4)
                {
                    GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = true);

                    

                    if (LinkedDoor.room.RoomSide == Room.Side.LEFT)
                    {
                        yield return StartCoroutine(
                            UIRoomTransition.current.StartTransition(UIRoomTransition.current.LeftTransition));
                        GameManager.Instance.SplitCameraLeftBehaviour.ChangeCameraState(ECameraBehaviourState.STILL, _playersInRange.Select(p=> p.gameObject).ToArray());
                        TP_Players(LinkedDoor.TpPoint);
                        TP_Camera(LinkedDoor.room);

                        yield return StartCoroutine(
                            UIRoomTransition.current.EndTransition(UIRoomTransition.current.LeftTransition));
                    }
                    else
                    {
                        yield return StartCoroutine(
                            UIRoomTransition.current.StartTransition(UIRoomTransition.current.RightTransition));
                        GameManager.Instance.SplitCameraRightBehaviour.ChangeCameraState(ECameraBehaviourState.STILL, _playersInRange.Select(p=> p.gameObject).ToArray());
                        TP_Players(LinkedDoor.TpPoint);
                        TP_Camera(LinkedDoor.room);

                        yield return StartCoroutine(
                            UIRoomTransition.current.EndTransition(UIRoomTransition.current.RightTransition));
                    }
                    

                    GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);

                    hub.RoomDoorLeft._isLocked = false;
                    hub.RoomDoorRight._isLocked = false;
                }
                else if(_linkedDoor.room is Vestibule)
                {
                    GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = true);
                    yield return StartCoroutine(
                        UIRoomTransition.current.StartTransition(UIRoomTransition.current.HubTransition));
                    
                    TP_Players(LinkedDoor.TpPoint);
                    TP_Camera(LinkedDoor.room);
                    
                    yield return StartCoroutine(
                        UIRoomTransition.current.EndTransition(UIRoomTransition.current.HubTransition));
                    GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);

                    _isLocked = true;
                    GameManager.Instance.CurrentGamePhase = GameManager.GamePhase.GUESS;
                }
            }
            else // IF PLAYERS IN ROOM
            {
                Room room = player.CurrentRoom;

                if (_linkedDoor.room is Hub)
                {
                    GameManager.Instance.OnBackToHubRefused?.Invoke(this);
                    yield break;
                }
                if (room.RoomSide == Room.Side.RIGHT)
                {
                    if (GameManager.Instance.RightPlayers.Count == _playersInRange.Count &&
                        _playersInRange.All(player => player.PlayerController.IsButtonHeld(PlayerController.EButtonType.INTERACT)))
                    {
                        GameManager.Instance.RightPlayers.ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = true);
                        yield return StartCoroutine(
                            UIRoomTransition.current.StartTransition(UIRoomTransition.current.RightTransition));
                        
                        TP_SidePlayers();
                        
                        yield return StartCoroutine(
                            UIRoomTransition.current.EndTransition(UIRoomTransition.current.RightTransition));
                        GameManager.Instance.RightPlayers.ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);
                    }
                }
                else if (room.RoomSide == Room.Side.LEFT)
                {
                    GameManager.Instance.LeftPlayers.ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = true);
                    if (GameManager.Instance.LeftPlayers.Count == _playersInRange.Count &&
                        _playersInRange.All(player => player.PlayerController.IsButtonHeld(PlayerController.EButtonType.INTERACT)))
                    {
                        yield return StartCoroutine(
                            UIRoomTransition.current.StartTransition(UIRoomTransition.current.LeftTransition));
                        
                        TP_SidePlayers();
                        
                        yield return StartCoroutine(
                            UIRoomTransition.current.EndTransition(UIRoomTransition.current.LeftTransition));
                    }
                    GameManager.Instance.LeftPlayers.ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);
                }
            }
        }
        else
        {
            OnLockedDoorInterract?.Invoke();
        }
    }

    public void TP_Players(Transform[] tpPoint) // TP  tous les joueurs qui intéragissent avec this porte
    {
        foreach(Player p in _playersInRange) /* if (p.PlayerController.IsButtonHeld(PlayerController.EButtonType.INTERACT))*/
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
