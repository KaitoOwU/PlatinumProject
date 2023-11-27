using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Statue : Puzzle, IResettable
{

    private bool _isRepaired;
    [SerializeField] private GameObject _statueArm;
    
    protected override void OnInteract(Player player)
    {
        if (player.HeldPickable == null)
            return;
        
        if (!_isRepaired && player.HeldPickable.ID == 10) // BRAS DE STATUE
        {
            player.HeldPickable = null;
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
