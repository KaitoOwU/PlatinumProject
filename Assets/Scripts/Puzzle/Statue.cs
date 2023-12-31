using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Statue : Puzzle, IResettable
{
        [HideInInspector] public UnityEvent OnRepaired;

    private bool _isRepaired;
    [SerializeField] private GameObject _statueArm;
    
    protected override void OnInteract(Player player)
    {
        if (player.HeldPickable == null)
            return;
        
        if (!_isRepaired && player.HeldPickable.ID == 10) // BRAS DE STATUE
        {
            OnRepaired?.Invoke();
            player.HeldPickable = null;
            _statueArm.SetActive(true);
            _isRepaired = true;
            Reactive.PuzzleCompleted();
        }
    }


    public void ResetAsDefault()
    {
        _statueArm.SetActive(false);
        _isRepaired = false;
    }
}
