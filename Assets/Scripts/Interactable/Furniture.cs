using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Furniture;

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
            transform.DOShakePosition(1f);
        }
    }
    protected override void OnPush(Player player)
    {
        Debug.Log("Push");
        if (_furnitureType == EFurnitureType.MOVABLE)
        {
            //verify if can move
            //Vector3 dir = Vector3Int.RoundToInt(transform.forward);
            Vector3 fwd = Vector3Int.RoundToInt(player.transform.TransformDirection(Vector3.forward));
            Debug.DrawRay(player.transform.position, fwd * 50, Color.green);
            //Debug.DrawRay(player.transform.position, 10*(player.transform.position + transform.rotation*transform.forward), Color.magenta, 10);
            RaycastHit hit;

            // Check if our raycast has hit furniture
            if (Physics.Raycast(player.transform.position, fwd, out hit, 50))
            {
                player.PlayerController.SwitchMoveState(PlayerController.EMoveState.PUSH, fwd.x != 0 ? new Vector3(1,0,0): new Vector3(0, 0, 1));

                Vector3 aimingDir = fwd;
                float angle = -Mathf.Atan2(aimingDir.z, aimingDir.x) * Mathf.Rad2Deg + 90.0f;
                angle = Mathf.Round(angle / 90.0f) * 90.0f;
                Quaternion qTo = Quaternion.AngleAxis(angle, Vector3.up);
                player.transform.rotation = qTo;
                //player.transform.eulerAngles = vec;


                transform.parent = player.transform;
            }
        }
    }
    protected override void OnPushCanceled(Player player)
    {
        Debug.Log("Stop Push");

        transform.parent = null;
        player.PlayerController.SwitchMoveState(PlayerController.EMoveState.NORMAL);
    }

}
