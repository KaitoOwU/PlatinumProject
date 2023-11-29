using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PressurePlateManager : MonoBehaviour, IPuzzleReactive
{
    private List<PressurePlate> _pressurePlates = new();
    
    private List<Vector3> _baseRotations = new();
    [SerializeField] private Transform[] _doorAnchors;
    [SerializeField] private RewardGenerator _rewardGenerator;
    [SerializeField] private bool _arePPlatesLinked;

    private void Awake()
    {
        _pressurePlates = GetComponentsInChildren<PressurePlate>().ToList();
        for (int i = 0; i < _doorAnchors.Length; i++)
        {
            _baseRotations.Add(_doorAnchors[i].rotation.eulerAngles);
        }
    }

    public void CheckIfValid()
    {
        if((_pressurePlates.All(plate => plate.IsActive)&&_arePPlatesLinked)|| (!_arePPlatesLinked && _pressurePlates.FindAll(plate => plate.IsActive).Count >= 1))
        {
            PuzzleCompleted();
        }
        else
        {
            UncompletePuzzle();
        }
    }

    public void PuzzleCompleted()
    {
        if (_rewardGenerator != null)
        {
            _rewardGenerator.PuzzleCompleted();
        }
        for (int i = 0; i < _doorAnchors.Length; i++)
        {
            _doorAnchors[i].DORotate(new Vector3(0, _baseRotations[i].y - 90, 0), 1.5f);
        }
    }

    private void UncompletePuzzle()
    {
        for (int i = 0; i < _doorAnchors.Length; i++)
        {
            _doorAnchors[i].DORotate(new Vector3(0, _baseRotations[i].y, 0), 1.5f);
        }
    }
}
