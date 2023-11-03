using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Furniture;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Furniture : Interactable
{
    #region Fields
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

    protected override void OnInteract(Player player)
    {
        if(_furnitureType == EFurnitureType.SEARCHABLE)
        {
            _3Dmodel.transform.DOShakePosition(1f, new Vector3(0.1f, 0, 0.1f));
            if(_clue != null)
            {
                GameManager.Instance.FoundClues.Add(_clue);
                Debug.Log("Found Clue !");
            }
            else
            {
                Debug.Log("No Clue Found!");
            }
        }
    }

    #region Push
    protected override void OnPush(Player player)
    {
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
                    float angle = -Mathf.Atan2(fwd.z, fwd.x) * Mathf.Rad2Deg + 90.0f;
                    angle = Mathf.Round(angle / 90.0f) * 90.0f;
                    foreach (var p in _playersPushing)
                    {
                        Physics.IgnoreCollision(_collider, p.GetComponent<Collider>(), true);
                        p.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                        p.PlayerController.SwitchMoveState(PlayerController.EMoveState.PUSH, fwd.x != 0 ? new Vector3(1,0,0): new Vector3(0, 0, 1));
                    }
                    transform.parent.parent = player.transform;
                }
            }
        }
    }
    protected override void OnPushCanceled(Player player)
    {
        _playersPushing.Remove(player);
        transform.parent.parent = null;
        foreach (var p in _playersPushing)
        {
            Physics.IgnoreCollision(_collider, p.GetComponent<Collider>(), false);
            p.PlayerController.SwitchMoveState(PlayerController.EMoveState.NORMAL);
        }
    }
#endregion

}
