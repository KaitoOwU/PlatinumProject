using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Furniture;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Furniture : Interactable
{
    public EFurnitureType FurnitureType => _furnitureType;
    [Header("--Type--")]
    [SerializeField] private EFurnitureType _furnitureType;
    [Header("--Ref--")]
    [SerializeField] private GameObject _3Dmodel;

    public enum EFurnitureType
    {
        MOVABLE,
        SEARCHABLE,
    }

    private void Start()
    {
        _3Dmodel.layer = LayerMask.NameToLayer("Furniture");
    }

    protected override void OnInteract(Player player)
    {
        if(_furnitureType == EFurnitureType.SEARCHABLE)
        {
            _3Dmodel.transform.DOShakePosition(1f, new Vector3(0.1f, 0, 0.1f)) ;
        }
    }
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
                Physics.IgnoreCollision(_collider, player.GetComponent<Collider>(), true);
                float angle = -Mathf.Atan2(fwd.z, fwd.x) * Mathf.Rad2Deg + 90.0f;
                angle = Mathf.Round(angle / 90.0f) * 90.0f;
                player.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                player.PlayerController.SwitchMoveState(PlayerController.EMoveState.PUSH, fwd.x != 0 ? new Vector3(1,0,0): new Vector3(0, 0, 1));
                transform.parent.parent = player.transform;
            }
        }
    }
    protected override void OnPushCanceled(Player player)
    {
        transform.parent.parent = null;
        Physics.IgnoreCollision(_collider, player.GetComponent<Collider>(), false);

        player.PlayerController.SwitchMoveState(PlayerController.EMoveState.NORMAL);
    }

}
