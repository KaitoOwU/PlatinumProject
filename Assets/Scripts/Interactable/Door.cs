using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static CameraBehaviour;
using DG.Tweening;

[System.Serializable]
public class Door : Interactable
{

    [HideInInspector] public UnityEvent OnChangeRoom;
    [HideInInspector] public UnityEvent OnLockedDoorInterract;
    [HideInInspector] public UnityEvent OnLeavingHub;

    [SerializeField] private Transform[] _tpPoint;
    [SerializeField] private DoorType _doorTypeValue;
    [SerializeField] private Door _linkedDoor;
    [SerializeField] private Room room;
    [SerializeField] private bool _isLocked;
    List<Corridor> _corridors;
    [SerializeField] private MeshRenderer _doormat;
    [SerializeField] private GameObject _doorModel;
    [SerializeField] private string _onInteractLockedVestibuleMessage;
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
        base.OnInteract(player);
        StartCoroutine(DoorInteraction(player));
    }

    private IEnumerator DoorInteraction(Player player)
    {
        if(_isLocked && _linkedDoor.room is Vestibule)
        {
            if (_message != null && _message.gameObject.activeSelf)
            {
                TutorialManager.Instance.HideBubble(_message, 0);
                _message = null;
            }
            if ((_message == null || !_message.gameObject.activeSelf) && _onInteractMessage != "")
            {
                _message = TutorialManager.Instance.ShowBubbleMessage(player.Index, transform, player.PlayerController.Inputs.ControllerIndex, _onInteractLockedVestibuleMessage, TutorialManager.E_DisplayStyle.FADE);
            }
        }
        if (!_isLocked && !_linkedDoor.IsLocked)
        {
            
            OnChangeRoom?.Invoke();
            if (player.CurrentRoom is Hub) // IF PLAYERS IN HUB
            {

//#if UNITY_EDITOR
//#else
                var playersInRoom = GameManager.Instance.PlayerList.FindAll(p => p.PlayerRef.CurrentRoom is Hub).ToList();
                if (playersInRoom.FindAll(player => player.PlayerController.IsButtonHeld(PlayerController.EButtonType.INTERACT)).Count != playersInRoom.Count)
                    yield break;
//#endif

                int countInHub = GameManager.Instance.PlayerList.FindAll(player => player.PlayerRef.RelativePos == HubRelativePosition.HUB).Count;
                int count = GameManager.Instance.PlayerList.FindAll(player => player.PlayerController.IsButtonHeld(PlayerController.EButtonType.INTERACT)).Count;

                Hub hub = (Hub)player.CurrentRoom;
                
//#if UNITY_EDITOR
//                if ((hub.RoomDoorRight.PlayersInRange.Count >= 1 || hub.RoomDoorLeft.PlayersInRange.Count >= 1) && count >= 1 && countInHub == 4) //TP FROM HUB AT GAME START
//                {
//#else
                if (hub.RoomDoorRight.PlayersInRange.Count >= 1 && hub.RoomDoorLeft.PlayersInRange.Count >= 1 && count == 4  && countInHub == 4 && hub.RoomDoorRight.PlayersInRange.Count + hub.RoomDoorLeft.PlayersInRange.Count == 4) 
                {
//#endif
                    OnLeavingHub?.Invoke();

                    GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p =>
                    {
                        p.PlayerController.Inputs.InputLocked = true;
                        p.PlayerController.Rigidbody.velocity = Vector3.zero;
                    });
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

                    GameManager.Instance.SplitCameraLeftBehaviour.ChangeCameraState(ECameraBehaviourState.STILL, _playersInRange.Select(p => p.gameObject).ToArray());
                    GameManager.Instance.SplitCameraRightBehaviour.ChangeCameraState(ECameraBehaviourState.STILL, _playersInRange.Select(p => p.gameObject).ToArray());

                    GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);
                    yield return StartCoroutine(
                        UIRoomTransition.current.EndTransition(UIRoomTransition.current.HubTransition));

   
                    hub.RoomDoorRight.Room.EnterRoom();
                    hub.RoomDoorLeft.Room.EnterRoom();
                }
                else if (_playersInRange.Count == countInHub && countInHub < 4) //TP FROM HUB AFTER SPLITING
                {
                    if (LinkedDoor.room.RoomSide == Room.Side.LEFT)
                    {
                        GameManager.Instance.LeftPlayers.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p =>
                        {
                            p.PlayerController.Inputs.InputLocked = true;
                            p.PlayerController.Rigidbody.velocity = Vector3.zero;
                        });
                        GameManager.Instance.SplitCameraLeftBehaviour.ChangeCameraState(ECameraBehaviourState.STILL, _playersInRange.Select(p=> p.gameObject).ToArray());
                        yield return StartCoroutine(
                            UIRoomTransition.current.StartTransition(UIRoomTransition.current.LeftTransition));
                        TP_Players(LinkedDoor.TpPoint);
                        TP_Camera(LinkedDoor.room);

                        yield return StartCoroutine(
                            UIRoomTransition.current.EndTransition(UIRoomTransition.current.LeftTransition));
                        GameManager.Instance.LeftPlayers.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);

                        LinkedDoor.Room.EnterRoom();

                    }
                    else
                    {
                        GameManager.Instance.RightPlayers.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p =>
                        {
                            p.PlayerController.Inputs.InputLocked = true;
                            p.PlayerController.Rigidbody.velocity = Vector3.zero;
                        });
                        GameManager.Instance.SplitCameraRightBehaviour.ChangeCameraState(ECameraBehaviourState.STILL, _playersInRange.Select(p=> p.gameObject).ToArray());
                        yield return StartCoroutine(
                            UIRoomTransition.current.StartTransition(UIRoomTransition.current.RightTransition));
                        TP_Players(LinkedDoor.TpPoint);
                        TP_Camera(LinkedDoor.room);

                        yield return StartCoroutine(
                            UIRoomTransition.current.EndTransition(UIRoomTransition.current.RightTransition));
                        GameManager.Instance.RightPlayers.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);

                        LinkedDoor.Room.EnterRoom();

                    }

                    hub.RoomDoorLeft._isLocked = false;
                    hub.RoomDoorRight._isLocked = false;
                }
                else if(_linkedDoor.room is Vestibule)
                {
                    if (_playersInRange.Count == 4 && _playersInRange.All(player => player.PlayerController.IsButtonHeld(PlayerController.EButtonType.INTERACT)))
                    {
                        GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p =>
                        {
                            p.PlayerController.Inputs.InputLocked = true;
                            p.PlayerController.Rigidbody.velocity = Vector3.zero;
                        });
                        yield return StartCoroutine(
                        UIRoomTransition.current.StartTransition(UIRoomTransition.current.HubTransition));
                    
                        TP_Players(LinkedDoor.TpPoint);
                        TP_Camera(LinkedDoor.room);
                    
                        yield return StartCoroutine(
                        UIRoomTransition.current.EndTransition(UIRoomTransition.current.HubTransition));
                        GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);

                        LinkedDoor.Room.EnterRoom();
                        _isLocked = true;
                        GameManager.Instance.CurrentGamePhase = GameManager.GamePhase.GUESS;
                    }                       

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
                        GameManager.Instance.RightPlayers.ToList().ForEach(p =>
                        {
                            p.PlayerController.Inputs.InputLocked = true;
                            p.PlayerController.Rigidbody.velocity = Vector3.zero;
                        });

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
                    GameManager.Instance.LeftPlayers.ToList().ForEach(p =>
                    {
                        p.PlayerController.Inputs.InputLocked = true;
                        p.PlayerController.Rigidbody.velocity = Vector3.zero;
                    });

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
            StartCoroutine(LockedFeedback());
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
                GameManager.Instance.TP_Camera(GameManager.Instance.FullCamera.gameObject, room.CameraPoint);
                break;
        }
    }
    
    public void UpdateRoom(Room room)
    {
        foreach(Player p in _playersInRange)
        {
            p.PlayerController.Animator.SetBool("IsMoving", false);
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
    private IEnumerator LockedFeedback()
    {
        _doorModel.transform.DOShakePosition(1f, new Vector3(0f, 0, 0.05f),20);
        yield return null;
            
    }
}
