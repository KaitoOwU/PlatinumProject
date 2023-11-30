using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlateManager : MonoBehaviour, IPuzzleReactive
{
    private List<PressurePlate> _pressurePlates = new();
    
    private List<float> _baseY = new();
    [SerializeField] private Transform[] _doorAnchors;
    [SerializeField] private RewardGenerator _rewardGenerator;
    [SerializeField] private bool _arePPlatesLinked;
    [HideInInspector] public UnityEvent OnDoorOpen;
    [HideInInspector] public UnityEvent OnDoorClose;

    private void Awake()
    {
        _pressurePlates = GetComponentsInChildren<PressurePlate>().ToList();
        for (int i = 0; i < _doorAnchors.Length; i++)
        {
            _baseY.Add(_doorAnchors[i].localPosition.y);
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
            Debug.Log("aa");
            _doorAnchors[i].DOMoveY(_baseY[i]-6, 1.5f);
        }
    }

    private void UncompletePuzzle()
    {
        for (int i = 0; i < _doorAnchors.Length; i++)
        {
            Debug.Log("aa1");
            _doorAnchors[i].DOMoveY(_baseY[i]+1.7f, 1.5f); ;
        }
    }
}
