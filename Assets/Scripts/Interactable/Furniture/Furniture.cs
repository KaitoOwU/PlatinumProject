using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;
using static Furniture;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Furniture : Interactable
{
    #region Fields


    [HideInInspector] public UnityEvent OnClueFoundInFurniture;
    [HideInInspector] public UnityEvent OnClueNotFoundInFurniture;

    [HideInInspector] public UnityEvent OnBeginPushingFurniture;
    [HideInInspector] public UnityEvent OnStopPushingFurniture;

    [HideInInspector] public UnityEvent OnBeginStrugglePushingFurniture;

    public EFurnitureType FurnitureType
    {
        get => _furnitureType;
        set => _furnitureType = value;
    }
    public int NeededPlayersCount
    {
        get => _neededPlayersCount;
        set => _neededPlayersCount = value;
    }
    public GameObject Model
    {
        get => _3Dmodel;
        set => _3Dmodel = value;
    }
    public Clue Clue
    {
        get => _clue;
        set => _clue = value;
    }
    public List<Player> PlayersPushing => _playersPushing;
    
    [Header("--Type--")]
    [SerializeField] private EFurnitureType _furnitureType;
    [Header("--Number of Players needed to push--")]
    //[ShowIf("showInt")]
    [SerializeField] private int _neededPlayersCount;
    [Header("--Ref--")]
    [SerializeField] private GameObject _3Dmodel;

    private List<Player> _playersPushing;
    private Clue _clue;

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
    }
    protected override void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exit " + other.gameObject.name);
        if (other.GetComponent<PlayerController>() == null)
            return;

        PlayerController p = other.GetComponent<PlayerController>();

        if (p.Inputs == null)
            return;

        _playersInRange.Remove(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);
        OnPushCanceled(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);


        p.Inputs.OnInteract?.RemoveListener(OnInteract);
        p.Inputs.OnPush?.RemoveListener(OnPush);
        p.Inputs.OnPushCanceled?.RemoveListener(OnPushCanceled);

        _onPlayerExitRange?.Invoke();
    }
    #endregion

    protected override void OnInteract(Player player)
    {
        if(_furnitureType == EFurnitureType.SEARCHABLE)
        {
            _3Dmodel.transform.DOShakePosition(1f, new Vector3(0.1f, 0, 0.1f));
            if(_clue != null)
            {
                GameManager.Instance.FoundClues.Add(_clue);
                OnClueFoundInFurniture?.Invoke();
                Debug.Log("Found Clue !");
            }
            else
            {
                OnClueNotFoundInFurniture?.Invoke();
                Debug.Log("No Clue Found!");
            }
        }
    }

    #region Push
    protected void OnPush(Player player)
    {
        if(_playersPushing.Contains(player))
            return;
        if (_furnitureType == EFurnitureType.MOVABLE)
        {
            Vector3 fwd = Vector3Int.RoundToInt(player.transform.TransformDirection(Vector3.forward));
            var x = Mathf.Max(Mathf.Abs(fwd.x), Mathf.Abs(fwd.z));
            fwd = x == Mathf.Abs(fwd.x) ? new Vector3(fwd.x, 0, 0) : new Vector3(0, 0, fwd.z);

            Debug.DrawRay(player.transform.position, fwd * 50, Color.green);

            // Check if our raycast has hit furniture
            if (Physics.Raycast(player.transform.position, fwd, 50, LayerMask.GetMask("Furniture")))
            {
                _playersPushing.Add(player);
                if(_playersPushing.Count >= _neededPlayersCount)
                {
                    OnBeginPushingFurniture?.Invoke();
                    float angle = -Mathf.Atan2(fwd.z, fwd.x) * Mathf.Rad2Deg + 90.0f;
                    angle = Mathf.Round(angle / 90.0f) * 90.0f;
                    foreach (var p in _playersPushing)
                    {
                        p.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                        p.PlayerController.SwitchMoveState(PlayerController.EMoveState.PUSH, fwd.x != 0 ? new Vector3(1,0,0): new Vector3(0, 0, 1));
                    }
                    transform.parent = player.transform;
                }
                else
                {
                    player.PlayerController.SwitchMoveState(PlayerController.EMoveState.PUSH_BLOCKED);
                    OnBeginStrugglePushingFurniture?.Invoke();
                }
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
            transform.parent = null;
        }
        else if(_playersPushing.Count < _neededPlayersCount)
        {
            foreach(var p in _playersPushing)
            {
                p.PlayerController.SwitchMoveState(PlayerController.EMoveState.PUSH_BLOCKED);
            }
        }
        player.PlayerController.SwitchMoveState(PlayerController.EMoveState.NORMAL);
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
}
