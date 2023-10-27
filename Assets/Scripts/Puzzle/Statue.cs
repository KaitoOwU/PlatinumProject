using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Statue : Interactable, IResettable
{

    private bool _isRepaired;
    [SerializeField] private GameObject _statueArm;
    
    protected override void OnInteract(Player player)
    {
        if (player.HeldItem == null)
            return;
        
        if (!_isRepaired && player.HeldItem.ID == 0) // BRAS DE STATUE
        {
            player.HeldItem = null;
            _statueArm.SetActive(true);
            _isRepaired = true;
        }
    }


    public void ResetAsDefault()
    {
        _statueArm.SetActive(false);
        _isRepaired = false;
    }
}
