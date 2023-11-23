using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PressurePlateManager : MonoBehaviour, IPuzzleReactive
{
    private List<PressurePlate> _pressurePlates = new();
    private Vector3 _baseRotation;
    [SerializeField] private Transform _doorAnchor;

    private void Awake()
    {
        _pressurePlates = GetComponentsInChildren<PressurePlate>().ToList();
        _baseRotation = _doorAnchor.rotation.eulerAngles;
    }

    public void CheckIfValid()
    {
        if(_pressurePlates.All(plate => plate.IsActive))
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
        _doorAnchor.DORotate(new Vector3(0, _baseRotation.y - 90, 0), 1.5f);
    }

    private void UncompletePuzzle()
    {
        _doorAnchor.DORotate(new Vector3(0, _baseRotation.y, 0), 1.5f);
    }
}
