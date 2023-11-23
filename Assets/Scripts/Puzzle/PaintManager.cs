using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintManager : Puzzle
{
    public Action<Paints> OnPuzzleUpdate;
    private PaintSoluce _soluce;
    private List<Paints> _paints = new();

    private void Start()
    {
        _paints = GetComponentsInChildren<Paints>().ToList();
        _soluce = GetComponentInParent<Room>().Tandem.GetComponentInChildren<PaintSoluce>();
    }

    private void OnEnable()
    {
        OnPuzzleUpdate += PuzzleUpdateCheck;
    }

    private void OnDisable()
    {
        OnPuzzleUpdate -= PuzzleUpdateCheck;
    }

    private void PuzzleUpdateCheck(Paints paint)
    {
        bool isComplete = true;
        Debug.Log(_soluce.PaintSelected.Count);
        Debug.Log(_paints.FindAll((paint) => paint.isTilted).Count);
        if (_paints.FindAll((paint) => paint.isTilted).Count == _soluce.PaintSelected.Count)
        {
            foreach(int id in _soluce.PaintSelected)
            {
                Debug.Log(id);
                if (!FindPaintWithID(id).isTilted)
                {
                    isComplete = false;
                }
            }
            Debug.Log(isComplete);
            if (isComplete)
            {
                Debug.Log("�a marche");
                Reactive.PuzzleCompleted();
            }
        }
    }
    private Paints FindPaintWithID(int ID)
    {
        foreach(Paints p in _paints)
        {
            if (p.Id == ID)
            {
                return p;
            }
        }
        return null;
    }
}
