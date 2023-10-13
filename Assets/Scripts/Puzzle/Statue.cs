using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Statue : Interactable
{

    private ReparationState _reparationState;
    [SerializeField] private GameObject _statueArm;
    
    protected override void OnInteract(Player player)
    {
        switch (_reparationState)
        {
            case ReparationState.NONE:

                if (player.HeldItem.ID == 1) //BRAS DE STATUE
                {
                    player.HeldItem = null;
                    _statueArm.SetActive(true);
                    _reparationState = ReparationState.ARM_PLACED;
                }
                
                break;
            
            case ReparationState.ARM_PLACED:

                if (player.HeldItem.ID == 0) //TOURNEVIS
                {
                    _statueArm.transform.DOLocalMoveZ(.6f, .5f);
                    _reparationState = ReparationState.REPAIRED;
                }
                
                break;
        }
    }
    
    private enum ReparationState {NONE, ARM_PLACED, REPAIRED}
}
