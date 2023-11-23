using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PressurePlateManager : MonoBehaviour, IPuzzleReactive
{
    private List<PressurePlate> _pressurePlates = new();
    
    private List<Vector3> _baseRotations;
    [SerializeField] private Transform[] _doorAnchors;

    private void Awake()
    {
        _pressurePlates = GetComponentsInChildren<PressurePlate>().ToList();
        for (int i = 0; i < _doorAnchors.Length; i++)
        {
            _baseRotations[i] = _doorAnchors[i].rotation.eulerAngles;
        }
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
