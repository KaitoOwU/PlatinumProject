using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PressurePlateManager : Puzzle
{
    public Action<PressurePlate> OnPuzzleUpdate;

    private List<PressurePlate> _pressurePlates = new();

    protected override void Awake()
    {
        base.Awake();
        _pressurePlates = GetComponentsInChildren<PressurePlate>().ToList();
    }

    private void OnEnable()
    {
        OnPuzzleUpdate += PuzzleUpdateCheck;
    }

    private void OnDisable()
    {
        OnPuzzleUpdate -= PuzzleUpdateCheck;
    }

    private void PuzzleUpdateCheck(PressurePlate plate)
    {
        if (_pressurePlates.FindAll((plate) => plate.IsValid).Count == _pressurePlates.Count)
        {
            _reactive.PuzzleCompleted();
        }
    }
}
