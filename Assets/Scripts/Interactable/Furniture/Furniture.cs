using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;
using static Furniture;
public class Furniture : Interactable
{
    #region Fields
    [HideInInspector] public UnityEvent OnSearchFurniture;
    [HideInInspector] public UnityEvent OnClueFoundInFurniture;
    [HideInInspector] public UnityEvent OnClueNotFoundInFurniture;

    [HideInInspector] public UnityEvent OnBeginPushingFurniture;
    [HideInInspector] public UnityEvent OnStopPushingFurniture;

    [HideInInspector] public UnityEvent OnBeginStrugglePushingFurniture;

    public EFurnitureType FurnitureType => _furnitureType;
    public GameObject Model => _3Dmodel;
    public int PlayersNeededNumber => _playersNeededNumber;
    public Clue Clue { get => _clue; set => _clue = value;}

    [SerializeField] private EFurnitureType _furnitureType;
    [SerializeField] private GameObject _3Dmodel;
    [SerializeField] private int _playersNeededNumber;
    [SerializeField] private Clue _clue;
    private Room _room;

    [SerializeField]
    private List<Player> _playersPushing;
    private float _baseY;
    private bool _searched = false;
    private Collider _furnitureModelCollider;
    [SerializeField]
    protected string _onTooHeavyMessage;

    #endregion
    public enum EFurnitureType
    {
        MOVABLE,
        SEARCHABLE,
    }

    private void Start()
    {
        _playersPushing = new();
        _3Dmodel.layer = LayerMask.NameToLayer("Furniture");
        _baseY = transform.position.y;
        _furnitureModelCollider = _3Dmodel.GetComponent<Collider>();
        _room = GetComponentInParent<Room>();
        GameManager.Instance.OnEachEndPhase.AddListener(ForceStopPush);

        switch (_furnitureType)
        {
            case EFurnitureType.MOVABLE:
                _onRangeMessage = "LR";
                break;
            case EFurnitureType.SEARCHABLE:
                _onRangeMessage = "A";
                break;
        }
    }

    #region Overridden methods
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;

        PlayerController p = other.GetComponent<PlayerController>();

        if (p.Inputs == null)
            return;

        _playersInRange.Add(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);

        _onPlayerEnterRange?.Invoke();

        p.Inputs.OnInteract.AddListener(OnInteract);
        p.Inputs.OnPush.AddListener(OnPush);
        p.Inputs.OnPushCanceled.AddListener(OnPushCanceled);

        if ((_message == null || !_message.gameObject.activeSelf) && _onRangeMessage != "")
        {
            _message = TutorialManager.Instance.ShowBubbleMessage(p.PlayerIndex, transform, p.Inputs.ControllerIndex, _onRangeMessage, TutorialManager.E_DisplayStyle.STAY);
        }

    }
    protected override void OnTriggerExit(Collider other)
    {
        //Debug.Log("Trigger exit " + other.gameObject.name);
        if (other.gameObject.layer == 8)
            return;
        if (other.GetComponent<PlayerController>() == null)
            return;

        PlayerController p = other.GetComponent<PlayerController>();

        if (p.Inputs == null)
            return;


        if(p.PushedFurniture != null && p.PushedFurniture == this)
            OnPushCanceled(other.GetComponent<Player>());
        _playersInRange.Remove(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);


        p.Inputs.OnInteract?.RemoveListener(OnInteract);
        p.Inputs.OnPush?.RemoveListener(OnPush);
        p.Inputs.OnPushCanceled?.RemoveListener(OnPushCanceled);

        _onPlayerExitRange?.Invoke();

        if (_message != null && _message.gameObject.activeSelf && _onRangeMessage != "")
        {
            TutorialManager.Instance.HideBubble(_message, 0);
        }
    }
    #endregion

    protected override void OnInteract(Player player)
    {
        base.OnInteract(player);

        if (_furnitureType == EFurnitureType.SEARCHABLE && !_searched)
        {
            _3Dmodel.transform.DOShakePosition(1f, new Vector3(0.1f, 0, 0.1f));
            OnSearchFurniture?.Invoke();
            if (_clue != null)
            {
                GameManager.Instance.FoundClues.Add(_clue);
                OnClueFoundInFurniture?.Invoke();
                UIItemFrameManager.instance.ClueAcquired(_clue.Data);
                if (player.RelativePos == HubRelativePosition.LEFT_WING /*Retirer*/|| player.RelativePos == HubRelativePosition.HUB)
                    UIClue.left.Init(_clue.Data);
                else if (player.RelativePos == HubRelativePosition.RIGHT_WING)
                    UIClue.right.Init(_clue.Data);
                Debug.Log("Found Clue !");
            }
            else
            {
                OnClueNotFoundInFurniture?.Invoke();
                Debug.Log("No Clue Found!");
            }
            _searched = true;
            transform.GetComponentInParent<Room>().CompletedRoom();
        }
    }

    #region Push
    protected void OnPush(Player player)
    {
        if (_message != null && _message.gameObject.activeSelf)
        {
            TutorialManager.Instance.HideBubble(_message, 0);
            _message = null;
        }
        if (_playersPushing.Contains(player))
            return;
        if (_furnitureType == EFurnitureType.MOVABLE)
        {
            Vector3 fwd = Vector3Int.RoundToInt(player.transform.TransformDirection(Vector3.forward));
            var x = Mathf.Max(Mathf.Abs(fwd.x), Mathf.Abs(fwd.z));
            fwd = x == Mathf.Abs(fwd.x) ? new Vector3(fwd.x, 0, 0) : new Vector3(0, 0, fwd.z);

            Debug.DrawRay(player.transform.position, fwd * 10, Color.green);
            RaycastHit hit = new RaycastHit();
            // Check if our raycast has hit furniture
            if (Physics.Raycast(player.transform.position + new Vector3(0,3,0), fwd * 10, out hit, 10, LayerMask.GetMask("Furniture")))
            {
                if (hit.collider == null)
                    return;                
                if (hit.collider != _furnitureModelCollider)
                    return;
                _playersPushing.Add(player);
                if (_playersPushing.Count >= _playersNeededNumber)
                {
                    OnBeginPushingFurniture?.Invoke();
                    float angle = -Mathf.Atan2(fwd.z, fwd.x) * Mathf.Rad2Deg + 90.0f;
                    angle = Mathf.Round(angle / 90.0f) * 90.0f;
                    player.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    //player.PlayerController.SwitchMoveState(PlayerController.EMoveState.PUSH, fwd.x != 0 ? new Vector3(1, 0, 0) : new Vector3(0, 0, 1));

                    foreach (var p in _playersPushing)
                    {
                        p.PlayerController.SwitchMoveState(PlayerController.EMoveState.PUSH, fwd.x != 0 ? new Vector3(1, 0, 0) : new Vector3(0, 0, 1));
                        p.PlayerController.PushedFurniture = this;
                    }
                }
                else
                {
                    player.PlayerController.SwitchMoveState(PlayerController.EMoveState.PUSH_BLOCKED);
                    OnBeginStrugglePushingFurniture?.Invoke();
                    if ((_message == null || !_message.gameObject.activeSelf) && _onTooHeavyMessage != "")
                    {
                        _message = TutorialManager.Instance.ShowBubbleMessage(player.Index, transform, player.PlayerController.Inputs.ControllerIndex, _onTooHeavyMessage, TutorialManager.E_DisplayStyle.FADE);
                    }
                }

                if(_playersPushing.Count == 1)
                    transform.parent = player.transform;
            }
        }
    }
    protected void OnPushCanceled(Player player)
    {
        _playersPushing.Remove(player);
        if (transform.parent == player.transform && _playersPushing.Count != 0)
        {
            transform.parent = _playersPushing[0].transform;
        }
        if (_playersPushing.Count == 0)
        {
            OnStopPushingFurniture?.Invoke();
            player.PlayerController.SwitchMoveState(PlayerController.EMoveState.NORMAL);
            player.PlayerController.Animator.SetBool("IsPushing", false);
            player.PlayerController.Animator.SetBool("IsPulling", false);
            transform.parent = _room.transform;
            transform.position = new Vector3(transform.position.x, _baseY, transform.position.z);

        }
        else if(_playersPushing.Count < _playersNeededNumber)
        {
            foreach(var p in _playersPushing)
            {
                p.PlayerController.SwitchMoveState(PlayerController.EMoveState.PUSH_BLOCKED);
            }
        }
        player.PlayerController.SwitchMoveState(PlayerController.EMoveState.NORMAL);
        player.PlayerController.Animator.SetBool("IsPushing", false);
        player.PlayerController.Animator.SetBool("IsPulling", false);

        
    }
    #endregion

    protected override void OnDestroy()
    {
        foreach (Player p in _playersInRange)
        {
            GameManager.Instance.PlayerList[p.Index - 1].PlayerController.Inputs.OnInteract?.RemoveListener(OnInteract);
            GameManager.Instance.PlayerList[p.Index - 1].PlayerController.Inputs.OnPush?.RemoveListener(OnPush);
            GameManager.Instance.PlayerList[p.Index - 1].PlayerController.Inputs.OnPushCanceled?.RemoveListener(OnPushCanceled);
        }
    }
    private void ForceStopPush()
    {
        transform.parent = _room.transform;
        for (var index = 0; index < _playersPushing.Count; index++)
        {
            try
            {
                Player p = _playersPushing[index];
                OnPushCanceled(p);
            }
            catch (IndexOutOfRangeException ex)
            {
                continue;
            }
        }
    }
}
