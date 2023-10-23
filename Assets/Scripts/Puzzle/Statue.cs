using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Statue : Interactable
{

    private bool _isRepaired;
    [SerializeField] private GameObject _statueArm;
    
    protected override void OnInteract(Player player)
    {
        if (!_isRepaired && player.HeldItem.ID == 1) // BRAS DE STATUE
        {
            player.HeldItem = null;
            _statueArm.SetActive(true);
            _isRepaired = true;
        }
    }
}
